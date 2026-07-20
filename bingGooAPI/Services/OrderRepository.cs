using System.Data;
using Dapper;
using JuJuBiAPI.Entities;
using JuJuBiAPI.Interfaces;
using JuJuBiAPI.Models.Order;

namespace JuJuBiAPI.Services
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IDbConnection _connection;

        public OrderRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        // ✅ Create Order (Manual Insert)
        public async Task<Order> CreateOrderAsync(Order order)
        {
            var sql = @"
                INSERT INTO Orders
                (
                    CartID,
                    UserID,
                    OutletID,
                    SubTotal,
                    DiscountAmount,
                    TaxAmount,
                    GrandTotal,
                    OrderStatus,
                    CreatedAt
                )
                VALUES
                (
                    @CartID,
                    @UserID,
                    @OutletID,
                    @SubTotal,
                    @DiscountAmount,
                    @TaxAmount,
                    @GrandTotal,
                    @OrderStatus,
                    GETDATE()
                );

                SELECT *
                FROM Orders
                WHERE OrderID = CAST(SCOPE_IDENTITY() AS INT);
            ";

            return await _connection
                .QuerySingleAsync<Order>(sql, order);
        }

        // ✅ Insert OrderItem
        public async Task AddOrderItemAsync(OrderItem item)
        {
            var sql = @"
                INSERT INTO OrderItems
                (
                    OrderID,
                    ProductID,
                    Quantity,
                    UnitPrice,
                    DiscountPercent,
                    DiscountAmount,
                    TaxPercent,
                    TaxAmount,
                    SubTotal,
                    TotalPrice,
                    CreatedAt
                )
                VALUES
                (
                    @OrderID,
                    @ProductID,
                    @Quantity,
                    @UnitPrice,
                    @DiscountPercent,
                    @DiscountAmount,
                    @TaxPercent,
                    @TaxAmount,
                    @SubTotal,
                    @TotalPrice,
                    GETDATE()
                );
            ";

            await _connection.ExecuteAsync(sql, item);
        }

        // ✅ Get Order + Items (with cashier/outlet and product names for display)
        public async Task<Order?> GetOrderByIdAsync(int orderId)
        {
            var sql = @"
                SELECT
                    O.*,
                    U.FullName  AS CashierName,
                    OT.OutletName
                FROM Orders O
                    LEFT JOIN Users U   ON U.Id = O.UserID
                    LEFT JOIN Outlet OT ON OT.Id = O.OutletID
                WHERE O.OrderID = @OrderID;

                SELECT
                    OI.*,
                    P.ProductName,
                    P.ProductCode,
                    P.ImageUrl
                FROM OrderItems OI
                    LEFT JOIN Products P ON P.ProductID = OI.ProductID
                WHERE OI.OrderID = @OrderID;
            ";

            using var multi =
                await _connection.QueryMultipleAsync(
                    sql,
                    new { OrderID = orderId });

            var order =
                await multi.ReadSingleOrDefaultAsync<Order>();

            if (order == null)
                return null;

            order.OrderItems =
                (await multi.ReadAsync<OrderItem>()).ToList();

            return order;
        }

        public async Task<List<Order>> GetOrdersByUserAsync(int userId)
        {
            var sql = @"
                SELECT *
                FROM Orders
                WHERE UserID = @UserID
                ORDER BY CreatedAt DESC;
            ";

            var orders = await _connection
                .QueryAsync<Order>(sql, new { UserID = userId });

            return orders.ToList();
        }

        public async Task<List<Order>> GetOrdersByOutletAsync(int outletId)
        {
            var sql = @"
                SELECT
                    O.*,
                    U.FullName  AS CashierName,
                    OT.OutletName
                FROM Orders O
                    LEFT JOIN Users U  ON U.Id = O.UserID
                    LEFT JOIN Outlet OT ON OT.Id = O.OutletID
                WHERE O.OutletID = @OutletID
                ORDER BY O.CreatedAt DESC;
            ";

            var orders = await _connection
                .QueryAsync<Order>(sql, new { OutletID = outletId });

            return orders.ToList();
        }

        // One-shot checkout for the outlet POS. Builds the server-side cart with
        // totals computed here (the Cart endpoints leave item totals at 0 — their
        // CalculateItem step is commented out), then reuses the CheckoutCart proc
        // so invoice numbering and the order insert stay in one place.
        public async Task<PosCheckoutResult> PosCheckoutAsync(PosCheckoutRequest request)
        {
            decimal subTotal = request.Items.Sum(i => i.UnitPrice * i.Quantity);

            // --- Stock guard: refuse to sell what the outlet doesn't have ---
            // Checked up front so the sale fails cleanly before anything is
            // written. (A products row may not exist yet for a first-time sale;
            // treat missing stock rows as zero.)
            foreach (var line in request.Items)
            {
                var available = await _connection.QuerySingleOrDefaultAsync<int?>(@"
                    SELECT ps.StockQty
                    FROM ProductStocks ps
                    JOIN Products p ON p.ProductID = ps.ProductID
                    WHERE p.ProductCode = @ProNumY AND ps.OutletId = @OutletId;",
                    new { line.ProNumY, OutletId = request.OutletId }) ?? 0;

                if (line.Quantity > available)
                {
                    var name = await _connection.QuerySingleOrDefaultAsync<string>(
                        "SELECT ProName FROM TPRProducts WHERE ProNumY = @ProNumY;",
                        new { line.ProNumY }) ?? line.ProNumY;

                    throw new InvalidOperationException(
                        $"Not enough stock for {name}: {available} left, {line.Quantity} requested.");
                }
            }

            // --- Loyalty: validate the redemption against the stored balance ---
            // The POS is trusted for prices (internal terminal) but never for the
            // points balance, since that is money the customer already owns.
            int redeemPoints = 0;
            decimal redeemValue = 0m;

            if (request.CustomerId.HasValue && request.RedeemPoints > 0)
            {
                var balance = await _connection.QuerySingleOrDefaultAsync<int?>(
                    "SELECT Points FROM Customer WHERE CustomerID = @Id AND IsActive = 1;",
                    new { Id = request.CustomerId.Value });

                if (balance == null)
                    throw new InvalidOperationException("Customer not found or inactive.");

                if (request.RedeemPoints > balance.Value)
                    throw new InvalidOperationException(
                        $"Customer has {balance.Value} point(s) but {request.RedeemPoints} were requested.");

                redeemPoints = request.RedeemPoints;

                // Never let a redemption exceed the value of the sale.
                redeemValue = Math.Min(Math.Max(request.RedeemValue, 0m), subTotal);
            }

            decimal grandTotal = subTotal - redeemValue;

            var cartId = await _connection.QuerySingleAsync<int>(@"
                INSERT INTO Carts
                    (UserID, OutletID, SubTotal, DiscountAmount, TaxAmount, GrandTotal, Status, CreatedAt)
                VALUES
                    (@UserID, @OutletID, @SubTotal, @Discount, 0, @GrandTotal, 'Active', GETDATE());

                SELECT CAST(SCOPE_IDENTITY() AS INT);",
                new
                {
                    UserID = request.UserId,
                    OutletID = request.OutletId,
                    SubTotal = subTotal,
                    Discount = redeemValue,
                    GrandTotal = grandTotal
                });

            foreach (var line in request.Items)
            {
                // CartItems.ProductID has a hard FK to Products, but the POS menu
                // keys on TPRProducts.ProNumY. Mirror the product into Products on
                // first sale (ProductCode = ProNumY) so checkout never depends on
                // a separate catalog sync having been run.
                var rows = await _connection.ExecuteAsync(@"
                    IF NOT EXISTS (SELECT 1 FROM Products WHERE ProductCode = @ProNumY)
                        INSERT INTO Products
                            (ProductCode, ProductName, CategoryId, ImageUrl,
                             CostPrice, SellPrice, DiscountPercent, TaxPercent,
                             Status, IsActive, CreatedAt)
                        SELECT
                            p.ProNumY, p.ProName, c.Id, p.ProImage,
                            ISNULL(p.ProImpPri, 0), ISNULL(p.ProUPrSE, 0), 0, 0,
                            1, 1, GETDATE()
                        FROM TPRProducts p
                        LEFT JOIN Category c
                            ON (c.CategoryCode = p.ProCat OR c.Id = TRY_CAST(p.ProCat AS INT))
                        WHERE p.ProNumY = @ProNumY;

                    INSERT INTO CartItems
                        (CartID, ProductID, Quantity, UnitPrice,
                         DiscountPercent, DiscountAmount, TaxPercent, TaxAmount,
                         SubTotal, TotalPrice, Note, CreatedAt)
                    SELECT
                        @CartID, p.ProductID, @Quantity, @UnitPrice,
                        0, 0, 0, 0,
                        @LineTotal, @LineTotal, @Note, GETDATE()
                    FROM Products p
                    WHERE p.ProductCode = @ProNumY;",
                    new
                    {
                        CartID = cartId,
                        line.Quantity,
                        line.UnitPrice,
                        LineTotal = line.UnitPrice * line.Quantity,
                        line.ProNumY,
                        line.Note
                    });

                if (rows == 0)
                    throw new InvalidOperationException(
                        $"Product '{line.ProNumY}' does not exist in the product catalog (TPRProducts).");
            }

            var result = await _connection.QuerySingleAsync<PosCheckoutResult>(
                "CheckoutCart",
                new { CartID = cartId },
                commandType: CommandType.StoredProcedure);

            // Money is taken at the till, so the sale is Paid the moment checkout
            // succeeds. Staff then move it Preparing -> Completed on the queue
            // screen as the drink is actually made. (The proc still writes
            // 'Pending', so correct it here rather than editing the shared proc.)
            await _connection.ExecuteAsync(@"
                UPDATE Orders
                SET OrderStatus   = 'Paid',
                    CustomerId    = @CustomerId,
                    PaymentMethod = @PaymentMethod,
                    ShiftId       = @ShiftId,
                    UpdatedAt     = GETDATE()
                WHERE OrderID = @OrderID;",
                new
                {
                    OrderID = result.OrderID,
                    CustomerId = request.CustomerId,
                    PaymentMethod = string.IsNullOrWhiteSpace(request.PaymentMethod) ? "Cash" : request.PaymentMethod,
                    ShiftId = request.ShiftId
                });

            // --- Loyalty: spend first, then earn on what was actually paid ---
            if (request.CustomerId.HasValue)
            {
                if (redeemPoints > 0)
                    await ApplyPointsAsync(
                        request.CustomerId.Value, -redeemPoints, "Redeem",
                        result.OrderID, request.OutletId,
                        $"Redeemed on {result.InvoiceNo} (-{redeemValue:0.00})");

                // 1 point per whole currency unit spent, after any redemption.
                int earned = (int)Math.Floor(grandTotal);

                if (earned > 0)
                    await ApplyPointsAsync(
                        request.CustomerId.Value, earned, "Earn",
                        result.OrderID, request.OutletId,
                        $"Earned on {result.InvoiceNo}");

                result.PointsEarned = earned;
                result.PointsRedeemed = redeemPoints;
                result.PointsBalance = await _connection.QuerySingleAsync<int>(
                    "SELECT Points FROM Customer WHERE CustomerID = @Id;",
                    new { Id = request.CustomerId.Value });
            }

            // Deduct the sold quantities from this outlet's stock. A missing
            // stock row is created at 0 first so the movement is still recorded
            // (may go negative — the POS shows it as out of stock, not blocked).
            foreach (var line in request.Items)
            {
                await _connection.ExecuteAsync(@"
                    IF NOT EXISTS (
                        SELECT 1 FROM ProductStocks ps
                        JOIN Products p ON p.ProductID = ps.ProductID
                        WHERE p.ProductCode = @ProNumY AND ps.OutletId = @OutletId)
                        INSERT INTO ProductStocks (ProductID, StockQty, OutletId, LastUpdated)
                        SELECT p.ProductID, 0, @OutletId, SYSDATETIME()
                        FROM Products p WHERE p.ProductCode = @ProNumY;

                    UPDATE ps
                    SET ps.StockQty = ps.StockQty - @Quantity,
                        ps.LastUpdated = SYSDATETIME()
                    FROM ProductStocks ps
                    JOIN Products p ON p.ProductID = ps.ProductID
                    WHERE p.ProductCode = @ProNumY AND ps.OutletId = @OutletId;",
                    new
                    {
                        line.ProNumY,
                        line.Quantity,
                        OutletId = request.OutletId
                    });
            }

            return result;
        }

        // ✅ Update Status
        public async Task UpdateOrderStatusAsync(int orderId, string status)
        {
            var sql = @"
                UPDATE Orders
                SET
                    OrderStatus = @Status,
                    UpdatedAt = GETDATE()
                WHERE OrderID = @OrderID;
            ";

            await _connection.ExecuteAsync(sql, new
            {
                OrderID = orderId,
                Status = status
            });
        }

        // Voiding is the undo of a sale, so it must put everything back the way
        // it was: status, this outlet's stock, and any loyalty points that were
        // earned or redeemed on the order.
        public async Task VoidOrderAsync(int orderId, string reason, string voidedBy)
        {
            var order = await _connection.QueryFirstOrDefaultAsync<Order>(
                "SELECT * FROM Orders WHERE OrderID = @OrderID;", new { OrderID = orderId });

            if (order == null)
                throw new InvalidOperationException("Order not found.");

            if (order.OrderStatus == "Cancelled" || order.OrderStatus == "Voided")
                throw new InvalidOperationException("Order is already voided.");

            // 1. Mark voided with the reason.
            await _connection.ExecuteAsync(@"
                UPDATE Orders
                SET OrderStatus = 'Cancelled',
                    VoidReason  = @Reason,
                    UpdatedAt   = GETDATE()
                WHERE OrderID = @OrderID;",
                new { OrderID = orderId, Reason = reason });

            // 2. Return the items to this outlet's stock.
            await _connection.ExecuteAsync(@"
                UPDATE ps
                SET ps.StockQty = ps.StockQty + oi.Quantity,
                    ps.LastUpdated = SYSDATETIME()
                FROM OrderItems oi
                JOIN ProductStocks ps
                    ON ps.ProductID = oi.ProductID AND ps.OutletId = @OutletId
                WHERE oi.OrderID = @OrderID;",
                new { OrderID = orderId, OutletId = order.OutletID });

            // 3. Reverse loyalty movements tied to this order (earned points are
            //    taken back, redeemed points are refunded), with audit rows.
            var pointMoves = await _connection.QueryAsync<(int CustomerId, int Points)>(@"
                SELECT CustomerId, Points
                FROM PointTransactions
                WHERE OrderId = @OrderID AND TxType IN ('Earn', 'Redeem');",
                new { OrderID = orderId });

            foreach (var move in pointMoves)
            {
                await ApplyPointsAsync(
                    move.CustomerId, -move.Points, "Adjust",
                    orderId, order.OutletID,
                    $"Void of {order.InvoiceNo} by {voidedBy}: {reason}");
            }
        }

        // Moves a customer's balance and records why, in one statement so the
        // balance and its audit row can never disagree. OutletId is stored on
        // every row so branches can be settled even though points are shared.
        private async Task ApplyPointsAsync(
            int customerId, int delta, string txType, int? orderId, int? outletId, string remark)
        {
            await _connection.ExecuteAsync(@"
                UPDATE Customer
                SET Points = Points + @Delta
                WHERE CustomerID = @CustomerId;

                INSERT INTO PointTransactions
                    (CustomerId, OrderId, OutletId, Points, TxType, BalanceAfter, Remark, CreatedAt)
                SELECT
                    @CustomerId, @OrderId, @OutletId, @Delta, @TxType, Points, @Remark, GETDATE()
                FROM Customer
                WHERE CustomerID = @CustomerId;",
                new
                {
                    CustomerId = customerId,
                    Delta = delta,
                    TxType = txType,
                    OrderId = orderId,
                    OutletId = outletId,
                    Remark = remark
                });
        }

        // ✅ Checkout Using Stored Procedure
        public async Task<int> CheckoutAsync(int cartId)
        {
            var orderId = await _connection
                .QuerySingleAsync<int>(
                    "CheckoutCart",
                    new { CartID = cartId },
                    commandType: CommandType.StoredProcedure
                );

            return orderId;
        }
    }
}
