using Auth_API.Application.Interfaces.Warehouse;
using Auth_API.Domain.Entities.Warehouses.Products;
using Microsoft.AspNetCore.Mvc;

namespace Auth_API.Controllers.Warehouse
{
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // GET api/warehouses/{warehouseId}/products/{productId}/orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetAll(
            int warehouseId,
            int productId)
        {
            // Qui puoi aggiungere un controllo che il productId
            // appartenga davvero al warehouseId, se vuoi.
            var list = await _orderService.GetAllAsync(productId);
            return Ok(list);
        }

        // GET api/warehouses/{warehouseId}/products/{productId}/orders/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Order>> GetById(
            int warehouseId,
            int productId,
            int id)
        {
            var order = await _orderService.GetByIdAsync(id, productId);
            if (order == null) return NotFound();
            return Ok(order);
        }

        // POST api/warehouses/{warehouseId}/products/{productId}/orders
        [HttpPost]
        public async Task<ActionResult<Order>> Create(
            int warehouseId,
            int productId,
            [FromBody] OrderRequest request)
        {
            var created = await _orderService.CreateAsync(request, productId);
            return CreatedAtAction(
                nameof(GetById),
                new { warehouseId, productId, id = created.Id },
                created);
        }

        // PUT api/warehouses/{warehouseId}/products/{productId}/orders/{id}
        [HttpPut("{id:int}")]
        public async Task<ActionResult<Order>> Update(
            int warehouseId,
            int productId,
            int id,
            [FromBody] OrderRequest request)
        {
            var updated = await _orderService.UpdateAsync(id, request, productId);
            return updated == null ? NotFound() : Ok(updated);
        }

        // DELETE api/warehouses/{warehouseId}/products/{productId}/orders/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(
            int warehouseId,
            int productId,
            int id)
        {
            var success = await _orderService.DeleteAsync(id, productId);
            return success ? NoContent() : NotFound();
        }

    }
}
