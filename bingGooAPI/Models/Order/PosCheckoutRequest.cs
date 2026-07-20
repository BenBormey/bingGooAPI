namespace JuJuBiAPI.Models.Order
{
    // One-shot checkout for the outlet POS: the terminal keeps its cart in
    // memory and submits the whole order at once, instead of building a
    // server-side cart row by row like the Cart endpoints do.
    public class PosCheckoutRequest
    {
        public int UserId { get; set; }

        public int OutletId { get; set; }

        public List<PosCheckoutLine> Items { get; set; } = new();

        // How the customer paid: Cash / Card / E-Wallet.
        public string? PaymentMethod { get; set; }

        // The cashier's open shift this sale belongs to.
        public int? ShiftId { get; set; }

        // Loyalty member attached to this sale (null = walk-in customer).
        public int? CustomerId { get; set; }

        // Points the cashier is spending on this sale. The server re-checks the
        // balance and the redemption value; it never trusts the client's maths.
        public int RedeemPoints { get; set; }

        // Value the redeemed points are worth, applied as a discount.
        public decimal RedeemValue { get; set; }
    }

    public class PosCheckoutLine
    {
        public string ProNumY { get; set; } = string.Empty;

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        // Free-text prep instruction for this line (sugar/ice level, toppings…).
        public string? Note { get; set; }
    }

    public class PosCheckoutResult
    {
        public int OrderID { get; set; }

        public string InvoiceNo { get; set; } = string.Empty;

        // Loyalty outcome, so the POS can print it and show the new balance.
        public int PointsEarned { get; set; }

        public int PointsRedeemed { get; set; }

        public int PointsBalance { get; set; }
    }
}
