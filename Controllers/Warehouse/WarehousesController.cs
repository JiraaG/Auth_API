using Auth_API.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Auth_API.Domain.Entities.Warehouses;
using Auth_API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Auth_API.Controllers.Warehouse
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class WarehousesController : ControllerBase
    {
        private readonly IWarehouseService _service;

        public WarehousesController(IWarehouseService service)
        {
            _service = service;
        }

        private string UserId => User.GetUserId();
        //private string Email => User.GetEmail();
        //private string FirstName => User.GetFirstName();

        [HttpGet]
        //public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync(UserId));
        public async Task<IActionResult> GetAll()
        {
            var warehouses = await _service.GetAllAsync(UserId);

            IEnumerable<WarehousesResponse> response = warehouses.Select(w => new WarehousesResponse
            {
                Id = w.Id,
                Name = w.Name,
                Description = w.Description,
                CreationDate = w.CreationDate,
                ModifiedDate = w.ModifiedDate,
            });

            return Ok(response);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            Warehouses warehouse = await _service.GetByIdAsync(id, UserId);
            if (warehouse is null) return NotFound();

            WarehousesResponse response = new WarehousesResponse
            {
                Id = warehouse.Id,
                Name = warehouse.Name,
                Description = warehouse.Description,
                CreationDate = warehouse.CreationDate,
                ModifiedDate = warehouse.ModifiedDate,
            };

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] WarehousesRequest warehouse)
        {
            Warehouses created = await _service.CreateAsync(warehouse, UserId);

            WarehousesResponse response = new WarehousesResponse
            {
                Id = created.Id,
                Name = created.Name,
                Description = created.Description,
                CreationDate = created.CreationDate,
                ModifiedDate = created.ModifiedDate,
            };

            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] WarehousesRequest warehouse)
        {
            Warehouses updated = await _service.UpdateAsync(id, warehouse, UserId);
            if (updated is null) return NotFound();

            WarehousesResponse response = new WarehousesResponse
            {
                Id = updated.Id,
                Name = updated.Name,
                Description = updated.Description,
                CreationDate = updated.CreationDate,
                ModifiedDate = updated.ModifiedDate,
            };

            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id, UserId);

            return deleted ? NoContent() : NotFound();
        }

    }
}
