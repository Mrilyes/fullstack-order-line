using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using order_orderline.Application.DTOs;
using order_orderline.Application.Services;

namespace order_orderline.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderLineController : ControllerBase
    {
        private readonly IOrderLineService _orderLineService;

        public OrderLineController(IOrderLineService orderLineService)
        {
            _orderLineService = orderLineService;
        }

        // GET: api/OrderLine
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var orderLines = await _orderLineService.GetAllOrderLinesAsync();
            if (orderLines == null || !orderLines.Any()) return NotFound("No order lines found.");

            return Ok(orderLines);
        }

        // GET: api/OrderLine/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var orderLine = await _orderLineService.GetOrderLineByIdAsync(id);
            if (orderLine == null) return NotFound($"OrderLine with ID {id} not found.");
            return Ok(orderLine);
        }

        // POST: api/OrderLine
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] OrderLineDto orderLineDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdOrderLine = await _orderLineService.AddOrderLineAsync(orderLineDto);

            // Use the generated OrderLineId for CreatedAtAction
            return CreatedAtAction(nameof(GetById), new { id = createdOrderLine.OrderLineId }, createdOrderLine);
        }

        // PUT: api/OrderLine/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] OrderLineDto orderLineDto)
        {
            if (id != orderLineDto.OrderLineId)
            {
                return BadRequest("OrderLineId mismatch between URL and body.");
            }

            try
            {
                await _orderLineService.UpdateOrderLineAsync(id, orderLineDto);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                // Log the exception for debugging
                Console.WriteLine(ex);
                return StatusCode(500, "An error occurred while updating the order line.");
            }
        }

        // DELETE: api/OrderLine/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existingOrderLine = await _orderLineService.GetOrderLineByIdAsync(id);
            if (existingOrderLine == null)
                return NotFound($"OrderLine with ID {id} not found.");

            await _orderLineService.DeleteOrderLineAsync(id);
            return NoContent();
        }
    }
}
