using bingGooAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace bingGooAPI.Controllers
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
