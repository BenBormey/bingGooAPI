using JuJuBiAPI.Entities;
using JuJuBiAPI.Interfaces;
using JuJuBiAPI.Models.Cart;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace JuJuBiAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartRepository _cartRepo;

        public CartController(ICartRepository cartRepo)
        {
            _cartRepo = cartRepo;
        }

   
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetCart(int userId)
        {
            var cart = await _cartRepo.GetActiveCartByUserAsync(userId);

            return Ok(cart);
        }

 
        [HttpPost("add")]
        public async Task<IActionResult> AddToCart(AddToCartRequest request)
        {
         
            var cart = await _cartRepo.GetActiveCartByUserAsync(request.UserId);

            if (cart == null)
            {
                cart = new Cart
                {
                    UserID = request.UserId,
                    OutletID = request.OutletId,
                    Status = "Active"
                };

                cart = await _cartRepo.CreateCartAsync(cart);
            }

            var existingItem = await _cartRepo.GetCartItemAsync(
                cart.CartID,
                request.ProductId
            );

            if (existingItem != null)
            {
                existingItem.Quantity += request.Quantity;

                await _cartRepo.UpdateCartItemAsync(existingItem);
            }
            else
            {
                var item = new CartItem
                {
                    CartID = cart.CartID,
                    ProductID = request.ProductId,
                    Quantity = request.Quantity,
                    UnitPrice = request.UnitPrice,

                    DiscountPercent = request.DiscountPercent,
                    TaxPercent = request.TaxPercent
                };

                await _cartRepo.AddCartItemAsync(item);
            }

            // Cart totals are maintained by CartRepository (UpdateCartTotal)
            // on every item add/update/remove.
            return Ok("Item added to cart");
        }


        [HttpPut("update")]
        public async Task<IActionResult> UpdateItem(UpdateCartItemRequest request)
        {
            if (request == null)
                return BadRequest("Request is null");

            var item = await _cartRepo.GetCartItemAsync(
                request.CartId,
                request.ProductId
            );

            if (item == null)
                return NotFound("Item not found");

            if (request.Quantity <= 0)
            {
                await _cartRepo.RemoveCartItemAsync(item);

                return Ok("Item removed");
            }

            item.Quantity = request.Quantity;

            await _cartRepo.UpdateCartItemAsync(item);

            return Ok("Item updated");
        }


       
        [HttpDelete("remove/{cartItemId}")]
        public async Task<IActionResult> RemoveItem(int cartItemId)
        {
            var item = new CartItem
            {
                CartItemID = cartItemId
            };

            await _cartRepo.RemoveCartItemAsync(item);

            return Ok("Item removed");
        }
    }
}
