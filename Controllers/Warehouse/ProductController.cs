using Auth_API.Application.Interfaces.Warehouse;
using Auth_API.Application.Services.Warehouse;
using Auth_API.Domain.Entities.Account;
using Auth_API.Domain.Entities.Warehouses;
using Auth_API.Domain.Entities.Warehouses.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;

namespace Auth_API.Controllers.Warehouse
{
    [ApiController]
    [Authorize]
    [Route("api/warehouses/{warehouseId:int}")]
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
        [HttpGet("products")]
        public async Task<IActionResult> GetAll([FromRoute] int warehouseId)
        {
            IEnumerable<Product> products = await _service.GetAllAsync(warehouseId);

            IEnumerable<ProductResponse> resonse = products.Select(p => new ProductResponse
            {
                Id = p.Id,
                Code = p.Code,
                Name = p.Name,
                Description = p.Description,
                Quantity = p.Quantity,
                QuantityUnit = p.QuantityUnit,
                TotalQuantity = p.TotalQuantity,
                PurchasePrice = p.PurchasePrice,
                PurchaseDate = p.PurchaseDate,
                IsSold = p.IsSold,
                SaleOrdersPrice = p.SaleOrdersPrice,
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
                    CreatedAt = o.CreatedAt,
                    ModifiedAt = o.ModifiedAt,
                })
            });

            return Ok(resonse);
        }

        // GET api/warehouses/{warehouseId}/product/{id}
        [HttpGet("product/{id:int}")]
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
                Quantity = product.Quantity,
                QuantityUnit = product.QuantityUnit,
                TotalQuantity = product.TotalQuantity,
                PurchasePrice = product.PurchasePrice,
                PurchaseDate = product.PurchaseDate,
                IsSold = product.IsSold,
                SaleOrdersPrice = product.SaleOrdersPrice,
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
                    CreatedAt = o.CreatedAt,
                    ModifiedAt = o.ModifiedAt,
                })
            };

            return Ok(resonse);
        }

        // POST api/warehouses/{warehouseId}/product
        [HttpPost("product")]
        public async Task<IActionResult> Create([FromRoute] int warehouseId, [FromBody] ProductRequest dto)
        {
            var products = await _service.GetAllAsync(warehouseId);

            if (products.Any(p => string.Equals(p.Code, dto.Code, StringComparison.OrdinalIgnoreCase)))
                return Conflict("Esiste già un prodotto con lo stesso codice in questo magazzino.");

            if (products.Any(p => string.Equals(p.Name, dto.Name, StringComparison.OrdinalIgnoreCase)))
                return Conflict("Esiste già un prodotto con lo stesso nome in questo magazzino.");

            Product created = await _service.CreateAsync(dto, warehouseId);

            ProductResponse resonse = new ProductResponse
            {
                Id = created.Id,
                Code = created.Code,
                Name = created.Name,
                Description = created.Description,
                Quantity = created.Quantity,
                QuantityUnit = created.QuantityUnit,
                TotalQuantity = created.TotalQuantity,
                PurchasePrice = created.PurchasePrice,
                PurchaseDate = created.PurchaseDate,
                IsSold = created.IsSold,
                SaleOrdersPrice = created.SaleOrdersPrice,
                Currency = created.Currency,
                CreatedAt = created.CreatedAt,
                ModifiedAt = created.ModifiedAt,
                CategoryId = created.CategoryId,
                //CategoryName = created.Category.Name,

                //Orders = created.Orders.Select(o => new OrderResponse
                //{
                //    Id = o.Id,
                //    Customer = o.Customer,
                //    Quantity = o.Quantity,
                //    QuantityUnit = o.QuantityUnit,
                //    UnitPrice = o.UnitPrice,
                //    TotalPrice = o.TotalPrice,
                //    OrderDate = o.OrderDate,
                //    FulfillmentDate = o.FulfillmentDate,
                //    CreatedAt = o.CreatedAt,
                //    ModifiedAt = o.ModifiedAt,
                //})
            };

            return Ok(resonse);
        }

        // PUT api/warehouses/{warehouseId}/product/{id}
        [HttpPut("product/{id:int}")]
        public async Task<IActionResult> Update(
            [FromRoute] int warehouseId,
            [FromRoute] int id,
            [FromBody] ProductRequest dto)
        {
            var products = await _service.GetAllAsync(warehouseId);

            if (products.Any(p => p.Id != id && string.Equals(p.Code, dto.Code, StringComparison.OrdinalIgnoreCase)))
                return Conflict("Esiste già un prodotto con lo stesso codice in questo magazzino.");

            if (products.Any(p => p.Id != id && string.Equals(p.Name, dto.Name, StringComparison.OrdinalIgnoreCase)))
                return Conflict("Esiste già un prodotto con lo stesso nome in questo magazzino.");

            var updated = await _service.UpdateAsync(id, dto, warehouseId);
            if (updated == null)
                return NotFound();

            ProductResponse resonse = new ProductResponse
            {
                Id = updated.Id,
                Code = updated.Code,
                Name = updated.Name,
                Description = updated.Description,
                Quantity = updated.Quantity,
                QuantityUnit = updated.QuantityUnit,
                TotalQuantity = updated.TotalQuantity,
                PurchasePrice = updated.PurchasePrice,
                PurchaseDate = updated.PurchaseDate,
                IsSold = updated.IsSold,
                SaleOrdersPrice = updated.SaleOrdersPrice,
                Currency = updated.Currency,
                CreatedAt = updated.CreatedAt,
                ModifiedAt = updated.ModifiedAt,
                CategoryId = updated.CategoryId,
                //CategoryName = updated.Category.Name,

                //Orders = updated.Orders.Select(o => new OrderResponse
                //{
                //    Id = o.Id,
                //    Customer = o.Customer,
                //    Quantity = o.Quantity,
                //    QuantityUnit = o.QuantityUnit,
                //    UnitPrice = o.UnitPrice,
                //    TotalPrice = o.TotalPrice,
                //    OrderDate = o.OrderDate,
                //    FulfillmentDate = o.FulfillmentDate,
                //    CreatedAt = o.CreatedAt,
                //    ModifiedAt = o.ModifiedAt,
                //})
            };

            return Ok(resonse);
        }

        // DELETE api/warehouses/{warehouseId}/product/{id}
        [HttpDelete("product/{id:int}")]
        public async Task<IActionResult> Delete(
            [FromRoute] int warehouseId,
            [FromRoute] int id)
        {
            var success = await _service.DeleteAsync(id, warehouseId);
            if (!success)
                return NotFound();

            return NoContent();
        }

        // api/warehouses/{warehouseId}/product/{id}/order/{id}
        [HttpGet("product/{idProduct:int}/order/{id:int}")]
        public async Task<ActionResult> GetOrderById(
            [FromRoute] int warehouseId,
            [FromRoute] int idProduct, 
            [FromRoute] int id)
        {
            var order = await _orderService.GetByIdAsync(id, idProduct);
            if (order == null)
                return NotFound();

            OrderResponse orderRequest = new OrderResponse
            {
                Id = order.Id,
                Customer = order.Customer,
                Quantity = order.Quantity,
                QuantityUnit = order.QuantityUnit,
                UnitPrice = order.UnitPrice,
                TotalPrice = order.TotalPrice,
                OrderDate = order.OrderDate,
                CreatedAt = order.CreatedAt,
                ModifiedAt = order.ModifiedAt
            };

            return Ok(orderRequest);
        }

        // CREATE ORDERS FOR A PRODUCT
        [HttpPost("product/{idProduct:int}/order")]
        public async Task<ActionResult<Order>> CreateOrder(
            [FromRoute] int warehouseId,
            [FromRoute] int idProduct,
            [FromBody] OrderRequest request)
        {
            var created = await _orderService.CreateAsync(request, idProduct);

            var orderResponse = new OrderResponse
            {
                Id = created.Id,
                Customer = created.Customer,
                Quantity = created.Quantity,
                QuantityUnit = created.QuantityUnit,
                UnitPrice = created.UnitPrice,
                TotalPrice = created.TotalPrice,
                OrderDate = created.OrderDate,
                CreatedAt = created.CreatedAt,
                ModifiedAt = created.ModifiedAt
            };

            return CreatedAtAction(
                nameof(GetById),
                new { warehouseId, idProduct, id = created.Id },
                orderResponse);
        }

        // PUT ORDERS FOR A PRODUCT
        [HttpPut("product/{idProduct:int}/order/{id:int}")]
        public async Task<ActionResult<Order>> UpdateOrder(
            [FromRoute] int warehouseId,
            [FromRoute] int idProduct,
            int id,
            [FromBody] OrderRequest request)
        {
            var updated = await _orderService.UpdateAsync(id, request, idProduct);

            var orderResponse = new OrderResponse
            {
                Id = updated.Id,
                Customer = updated.Customer,
                Quantity = updated.Quantity,
                QuantityUnit = updated.QuantityUnit,
                UnitPrice = updated.UnitPrice,
                TotalPrice = updated.TotalPrice,
                OrderDate = updated.OrderDate,
                CreatedAt = updated.CreatedAt,
                ModifiedAt = updated.ModifiedAt
            };

            return updated == null ? NotFound() : Ok(orderResponse);
        }

        // DELETE ORDERS FOR A PRODUCT
        [HttpDelete("product/{idProduct:int}/order/{id:int}")]
        public async Task<IActionResult> Delete(
            [FromRoute] int warehouseId,
            [FromRoute] int idProduct,
            int id)
        {
            var success = await _orderService.DeleteAsync(id, idProduct);
            return success ? NoContent() : NotFound();
        }

    }
}
