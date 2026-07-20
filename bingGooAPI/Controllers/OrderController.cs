using JuJuBiAPI.Interfaces;
using JuJuBiAPI.Models.Order;
using Microsoft.AspNetCore.Mvc;

namespace JuJuBiAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepo;

        public OrderController(IOrderRepository orderRepo)
        {
            _orderRepo = orderRepo;
        }


        // POST: api/Order/pos-checkout — one-shot order from the outlet POS terminal
        [HttpPost("pos-checkout")]
        public async Task<IActionResult> PosCheckout(PosCheckoutRequest request)
        {
            if (request.UserId <= 0 || request.OutletId <= 0)
                return BadRequest("UserId and OutletId are required.");

            if (request.Items == null || request.Items.Count == 0)
                return BadRequest("Order has no items.");

            if (request.Items.Any(i => i.Quantity <= 0 || string.IsNullOrWhiteSpace(i.ProNumY)))
                return BadRequest("Every item needs a product code and a quantity above zero.");

            try
            {
                var result = await _orderRepo.PosCheckoutAsync(request);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "Checkout failed",
                    Error = ex.Message
                });
            }
        }

        public class VoidOrderRequest
        {
            public string Reason { get; set; } = string.Empty;
            public string? VoidedBy { get; set; }
        }

        // POST: api/Order/5/void — undo a sale: restores stock and loyalty points.
        [HttpPost("{orderId:int}/void")]
        public async Task<IActionResult> VoidOrder(int orderId, VoidOrderRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Reason))
                return BadRequest("A reason is required to void an order.");

            try
            {
                await _orderRepo.VoidOrderAsync(orderId, request.Reason.Trim(), request.VoidedBy ?? "unknown");
                return Ok(new { Message = "Order voided; stock and points restored." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/Order/outlet/3 — order history for one outlet's POS
        [HttpGet("outlet/{outletId:int}")]
        public async Task<IActionResult> GetOutletOrders(int outletId)
        {
            var orders = await _orderRepo.GetOrdersByOutletAsync(outletId);

            return Ok(orders);
        }

        [HttpPost("checkout/{cartId}")]
        public async Task<IActionResult> Checkout(int cartId)
        {
            if (cartId <= 0)
                return BadRequest("Invalid cart id");

            try
            {
                var orderId = await _orderRepo.CheckoutAsync(cartId);

                return Ok(new
                {
                    Message = "Checkout success",
                    OrderID = orderId
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "Checkout failed",
                    Error = ex.Message
                });
            }
        }


        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrder(int orderId)
        {
            var order = await _orderRepo.GetOrderByIdAsync(orderId);

            if (order == null)
                return NotFound("Order not found");

            return Ok(order);
        }

   
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserOrders(int userId)
        {
            var orders = await _orderRepo.GetOrdersByUserAsync(userId);

            return Ok(orders);
        }


    
        [HttpPut("status/{orderId}")]
        public async Task<IActionResult> UpdateStatus(
            int orderId,
            [FromQuery] string status)
        {
            if (string.IsNullOrEmpty(status))
                return BadRequest("Status is required");

            await _orderRepo.UpdateOrderStatusAsync(orderId, status);

            return Ok("Status updated");
        }
    }
}
