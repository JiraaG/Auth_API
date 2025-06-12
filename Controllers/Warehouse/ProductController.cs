using Auth_API.Application.Interfaces.Warehouse;
using Auth_API.Application.Services.Warehouse;
using Auth_API.Domain.Entities.Account;
using Auth_API.Domain.Entities.Warehouses;
using Auth_API.Domain.Entities.Warehouses.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Auth_API.Controllers.Warehouse
{
    [ApiController]
    [Authorize]
    [Route("api/warehouses/{warehouseId:int}/products")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _service;
        private readonly IOrderService _orderService;

        public ProductController(IProductService service, IOrderService orderService)
        {
            _service = service;
            _orderService = orderService;
        }

        // GET api/warehouses/{warehouseId}/products
        [HttpGet]
        public async Task<IActionResult> GetAll([FromRoute] int warehouseId)
        {
            IEnumerable<Product> products = await _service.GetAllAsync(warehouseId);

            IEnumerable<ProductResponse> resonse = products.Select(p => new ProductResponse
            {
                Id = p.Id,
                Code = p.Code,
                Name = p.Name,
                Description = p.Description,
                PurchasePrice = p.PurchasePrice,
                PurchaseDate = p.PurchaseDate,
                IsActive = p.IsActive,
                IsSold = p.IsSold,
                SalePrice = p.SalePrice,
                SaleDate = p.SaleDate,
                Currency = p.Currency,
                CreatedAt = p.CreatedAt,
                ModifiedAt = p.ModifiedAt,
                CategoryId = p.CategoryId,
                CategoryName = p.Category.Name,

                Orders = p.Orders.Select(o => new OrderResponse
                {
                    Id = o.Id,
                    Customer = o.Customer,
                    Quantity = o.Quantity,
                    QuantityUnit = o.QuantityUnit,
                    UnitPrice = o.UnitPrice,
                    TotalPrice = o.TotalPrice,
                    OrderDate = o.OrderDate,
                    FulfillmentDate = o.FulfillmentDate,
                    CreatedAt = o.CreatedAt,
                    ModifiedAt = o.ModifiedAt,
                })
            });

            return Ok(resonse);
        }

        // GET api/warehouses/{warehouseId}/products/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int warehouseId, [FromRoute] int id)
        {
            Product product = await _service.GetByIdAsync(id, warehouseId);
            if (product == null)
                return NotFound();

            ProductResponse resonse = new ProductResponse
            {
                Id = product.Id,
                Code = product.Code,
                Name = product.Name,
                Description = product.Description,
                PurchasePrice = product.PurchasePrice,
                PurchaseDate = product.PurchaseDate,
                IsActive = product.IsActive,
                IsSold = product.IsSold,
                SalePrice = product.SalePrice,
                SaleDate = product.SaleDate,
                Currency = product.Currency,
                CreatedAt = product.CreatedAt,
                ModifiedAt = product.ModifiedAt,
                CategoryId = product.CategoryId,
                CategoryName = product.Category.Name,

                Orders = product.Orders.Select(o => new OrderResponse
                {
                    Id = o.Id,
                    Customer = o.Customer,
                    Quantity = o.Quantity,
                    QuantityUnit = o.QuantityUnit,
                    UnitPrice = o.UnitPrice,
                    TotalPrice = o.TotalPrice,
                    OrderDate = o.OrderDate,
                    FulfillmentDate = o.FulfillmentDate,
                    CreatedAt = o.CreatedAt,
                    ModifiedAt = o.ModifiedAt,
                })
            };

            return Ok(resonse);
        }

        // POST api/warehouses/{warehouseId}/products
        [HttpPost]
        public async Task<IActionResult> Create([FromRoute] int warehouseId, [FromBody] ProductRequest dto)
        {
            Product created = await _service.CreateAsync(dto, warehouseId);

            ProductResponse resonse = new ProductResponse
            {
                Id = created.Id,
                Code = created.Code,
                Name = created.Name,
                Description = created.Description,
                PurchasePrice = created.PurchasePrice,
                PurchaseDate = created.PurchaseDate,
                IsActive = created.IsActive,
                IsSold = created.IsSold,
                SalePrice = created.SalePrice,
                SaleDate = created.SaleDate,
                Currency = created.Currency,
                CreatedAt = created.CreatedAt,
                ModifiedAt = created.ModifiedAt,
                CategoryId = created.CategoryId,
                CategoryName = created.Category.Name,

                Orders = created.Orders.Select(o => new OrderResponse
                {
                    Id = o.Id,
                    Customer = o.Customer,
                    Quantity = o.Quantity,
                    QuantityUnit = o.QuantityUnit,
                    UnitPrice = o.UnitPrice,
                    TotalPrice = o.TotalPrice,
                    OrderDate = o.OrderDate,
                    FulfillmentDate = o.FulfillmentDate,
                    CreatedAt = o.CreatedAt,
                    ModifiedAt = o.ModifiedAt,
                })
            };

            return Ok(resonse);
        }

        // PUT api/warehouses/{warehouseId}/products/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            [FromRoute] int warehouseId,
            [FromRoute] int id,
            [FromBody] ProductRequest dto)
        {
            var updated = await _service.UpdateAsync(id, dto, warehouseId);
            if (updated == null)
                return NotFound();

            ProductResponse resonse = new ProductResponse
            {
                Id = updated.Id,
                Code = updated.Code,
                Name = updated.Name,
                Description = updated.Description,
                PurchasePrice = updated.PurchasePrice,
                PurchaseDate = updated.PurchaseDate,
                IsActive = updated.IsActive,
                IsSold = updated.IsSold,
                SalePrice = updated.SalePrice,
                SaleDate = updated.SaleDate,
                Currency = updated.Currency,
                CreatedAt = updated.CreatedAt,
                ModifiedAt = updated.ModifiedAt,
                CategoryId = updated.CategoryId,
                CategoryName = updated.Category.Name,

                Orders = updated.Orders.Select(o => new OrderResponse
                {
                    Id = o.Id,
                    Customer = o.Customer,
                    Quantity = o.Quantity,
                    QuantityUnit = o.QuantityUnit,
                    UnitPrice = o.UnitPrice,
                    TotalPrice = o.TotalPrice,
                    OrderDate = o.OrderDate,
                    FulfillmentDate = o.FulfillmentDate,
                    CreatedAt = o.CreatedAt,
                    ModifiedAt = o.ModifiedAt,
                })
            };

            return Ok(resonse);
        }

        // DELETE api/warehouses/{warehouseId}/products/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(
            [FromRoute] int warehouseId,
            [FromRoute] int id)
        {
            var success = await _service.DeleteAsync(id, warehouseId);
            if (!success)
                return NotFound();

            return NoContent();
        }

        // CREATE ORDERS FOR A PRODUCT
        [HttpPost("order")]
        public async Task<ActionResult<Order>> CreateOrder(
            [FromRoute] int warehouseId,
            int productId,
            [FromBody] OrderRequest request)
        {
            var created = await _orderService.CreateAsync(request, productId);
            return CreatedAtAction(
                nameof(GetById),
                new { warehouseId, productId, id = created.Id },
                created);
        }

        // PUT ORDERS FOR A PRODUCT
        [HttpPut("order")]
        public async Task<ActionResult<Order>> UpdateOrder(
            [FromRoute] int warehouseId,
            int productId,
            int id,
            [FromBody] OrderRequest request)
        {
            var updated = await _orderService.UpdateAsync(id, request, productId);
            return updated == null ? NotFound() : Ok(updated);
        }

        // DELETE ORDERS FOR A PRODUCT
        [HttpDelete("order")]
        public async Task<IActionResult> Delete(
            [FromRoute] int warehouseId,
            int productId,
            int id)
        {
            var success = await _orderService.DeleteAsync(id, productId);
            return success ? NoContent() : NotFound();
        }

    }
}
