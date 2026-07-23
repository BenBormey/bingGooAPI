using JuJuBiAPI.Interfaces;
using JuJuBiAPI.Models.PurchaseOrder;
using JuJuBiAPI.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace JuJuBiAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseOrderController : ControllerBase
    {
        private readonly IPurchaseOrderRepository _repo;

        public PurchaseOrderController(IPurchaseOrderRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var purchaseOrders = await _repo.GetAllAsync();
            return Ok(purchaseOrders);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var purchaseOrder = await _repo.GetByIdAsync(id);

            if (purchaseOrder == null)
                return NotFound("Purchase order not found");

            return Ok(purchaseOrder);
        }

        [HttpGet("supplier/{supplierId}")]
        public async Task<IActionResult> GetBySupplier(int supplierId)
        {
            var purchaseOrders = await _repo.GetBySupplierAsync(supplierId);
            return Ok(purchaseOrders);
        }

        [HttpGet("outlet/{outletId}")]
        public async Task<IActionResult> GetByOutlet(int outletId)
        {
            var purchaseOrders = await _repo.GetByOutletAsync(outletId);
            return Ok(purchaseOrders);
        }

        [PermissionAuthorize("PURCHASE_ORDER")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePurchaseOrderDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var purchaseOrder = await _repo.CreateAsync(dto);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = purchaseOrder.PurchaseOrderID },
                    purchaseOrder);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "Failed to create purchase order",
                    Error = ex.Message
                });
            }
        }

        [PermissionAuthorize("PURCHASE_ORDER")]
        [HttpPut("status/{id}")]
        public async Task<IActionResult> UpdateStatus(int id, [FromQuery] string status)
        {
            if (string.IsNullOrEmpty(status))
                return BadRequest("Status is required");

            var updated = await _repo.UpdateStatusAsync(id, status);

            if (!updated)
                return NotFound("Purchase order not found");

            return Ok("Status updated");
        }

        [PermissionAuthorize("PURCHASE_ORDER")]
        [HttpPost("receive/{id}")]
        public async Task<IActionResult> Receive(int id, [FromBody] ReceivePurchaseOrderDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var received = await _repo.ReceiveAsync(id, dto);

                if (!received)
                    return NotFound("Purchase order not found");

                var purchaseOrder = await _repo.GetByIdAsync(id);

                return Ok(new
                {
                    Message = "Purchase order received",
                    PurchaseOrder = purchaseOrder
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "Failed to receive purchase order",
                    Error = ex.Message
                });
            }
        }

        [PermissionAuthorize("PURCHASE_ORDER")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var purchaseOrder = await _repo.GetByIdAsync(id);

            if (purchaseOrder == null)
                return NotFound(new { Success = false, Message = "Purchase order not found." });

            if (purchaseOrder.Status is "Received" or "PartiallyReceived")
                return BadRequest(new
                {
                    Success = false,
                    Message = "Cannot delete a purchase order that has already received stock."
                });

            await _repo.DeleteAsync(id);

            return Ok(new { Success = true, Message = "Purchase order deleted successfully." });
        }
    }
}
