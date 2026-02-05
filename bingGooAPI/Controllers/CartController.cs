using bingGooAPI.Entities;
using bingGooAPI.Interfaces;
using bingGooAPI.Models.Cart;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace bingGooAPI.Controllers
{
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

            if (cart == null)
                return NotFound("No active cart found");

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

                CalculateItem(existingItem);

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

                CalculateItem(item);

                await _cartRepo.AddCartItemAsync(item);
            }

  
            await RecalculateCart(cart.CartID);

            return Ok("Item added to cart");
        }


        [HttpPut("update")]
        public async Task<IActionResult> UpdateItem(UpdateCartItemRequest request)
        {
            if (request == null)
                return BadRequest("Request is null");
            int userId = int.Parse(
     User.FindFirstValue(ClaimTypes.NameIdentifier)
 );
            var item = await _cartRepo.GetCartItemAsync(
                request.CartId,
                request.ProductId
            );

            if (item == null)
                return NotFound("Item not found");

            // ✅ If qty <= 0 → remove
            if (request.Quantity <= 0)
            {
                await _cartRepo.RemoveCartItemAsync(item);

                await RecalculateCart(userId);

                return Ok("Item removed");
            }

   
            item.Quantity = request.Quantity;

            CalculateItem(item);

            await _cartRepo.UpdateCartItemAsync(item);

            await RecalculateCart(item.CartID);

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

  

        private void CalculateItem(CartItem item)
        {
            item.SubTotal = item.Quantity * item.UnitPrice;

            item.DiscountAmount =
                item.SubTotal * item.DiscountPercent / 100;

            var afterDiscount = item.SubTotal - item.DiscountAmount;

            item.TaxAmount =
                afterDiscount * item.TaxPercent / 100;

            item.TotalPrice =
                afterDiscount + item.TaxAmount;
        }

        private async Task RecalculateCart(int cartId)
        {
            var cart = await _cartRepo.GetActiveCartByUserAsync(cartId);

            if (cart == null) return;

            cart.SubTotal = cart.CartItems.Sum(x => x.SubTotal);
            cart.DiscountAmount = cart.CartItems.Sum(x => x.DiscountAmount);
            cart.TaxAmount = cart.CartItems.Sum(x => x.TaxAmount);
            cart.GrandTotal = cart.CartItems.Sum(x => x.TotalPrice);

            await _cartRepo.UpdateCartAsync(cart);
        }
    }






}
