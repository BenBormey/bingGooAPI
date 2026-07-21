using JuJuBiAPI.Interfaces;
using JuJuBiAPI.Models.TransferOrder;
using Microsoft.AspNetCore.Mvc;

namespace JuJuBiAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransferOrderController : ControllerBase
    {
        private readonly ITransferOrderRepository _repo;

        public TransferOrderController(ITransferOrderRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var transferOrders = await _repo.GetAllAsync();
            return Ok(transferOrders);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var transferOrder = await _repo.GetByIdAsync(id);

            if (transferOrder == null)
                return NotFound("Transfer order not found");

            return Ok(transferOrder);
        }

        [HttpGet("outlet/{outletId}")]
        public async Task<IActionResult> GetByOutlet(int outletId)
        {
            var transferOrders = await _repo.GetByOutletAsync(outletId);
            return Ok(transferOrders);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTransferOrderDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var transferOrder = await _repo.CreateAsync(dto);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = transferOrder.TransferOrderId },
                    transferOrder);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "Failed to create transfer order",
                    Error = ex.Message
                });
            }
        }

        [HttpPut("status/{id}")]
        public async Task<IActionResult> UpdateStatus(int id, [FromQuery] string status)
        {
            if (string.IsNullOrEmpty(status))
                return BadRequest("Status is required");

            var updated = await _repo.UpdateStatusAsync(id, status);

            if (!updated)
                return NotFound("Transfer order not found");

            return Ok("Status updated");
        }

        // Ship: deducts stock from the source outlet.
        [HttpPost("approve/{id}")]
        public async Task<IActionResult> Approve(int id, [FromBody] ApproveTransferOrderDto dto)
        {
            try
            {
                var approved = await _repo.ApproveAsync(id, dto);

                if (!approved)
                    return NotFound("Transfer order not found");

                var transferOrder = await _repo.GetByIdAsync(id);

                return Ok(new
                {
                    Message = "Transfer order approved",
                    TransferOrder = transferOrder
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
                    Message = "Failed to approve transfer order",
                    Error = ex.Message
                });
            }
        }

        // Receive: adds stock to the destination outlet.
        [HttpPost("receive/{id}")]
        public async Task<IActionResult> Receive(int id, [FromBody] ReceiveTransferOrderDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var received = await _repo.ReceiveAsync(id, dto);

                if (!received)
                    return NotFound("Transfer order not found");

                var transferOrder = await _repo.GetByIdAsync(id);

                return Ok(new
                {
                    Message = "Transfer order received",
                    TransferOrder = transferOrder
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
                    Message = "Failed to receive transfer order",
                    Error = ex.Message
                });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var transferOrder = await _repo.GetByIdAsync(id);

            if (transferOrder == null)
                return NotFound(new { Success = false, Message = "Transfer order not found." });

            if (transferOrder.Status is "Approved" or "Received" or "PartiallyReceived")
                return BadRequest(new
                {
                    Success = false,
                    Message = "Cannot delete a transfer order that has already been approved or received."
                });

            await _repo.DeleteAsync(id);

            return Ok(new { Success = true, Message = "Transfer order deleted successfully." });
        }
    }
}
