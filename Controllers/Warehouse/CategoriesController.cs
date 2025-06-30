using Auth_API.Application.Interfaces.Warehouse;
using Auth_API.Domain.Entities.Warehouses.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Auth_API.Controllers.Warehouse
{
    [ApiController]
    [Authorize]
    [Route("api/categories")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _svc;
        public CategoriesController(ICategoryService svc) => _svc = svc;

        // GET /api/categories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryResponse>>> GetAll()
        {
            var list = await _svc.GetAllAsync();
            return Ok(list);
        }

        // GET /api/categories/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<CategoryResponse>> GetById(int id)
        {
            var cat = await _svc.GetByIdAsync(id);
            return cat == null ? NotFound() : Ok(cat);
        }

        // POST /api/categories
        [HttpPost]
        public async Task<ActionResult<CategoryResponse>> Create([FromBody] CategoryRequest req)
        {
            var created = await _svc.CreateAsync(req);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT /api/categories/5
        [HttpPut("{id:int}")]
        public async Task<ActionResult<CategoryResponse>> Update(
            int id,
            [FromBody] CategoryRequest req)
        {
            var updated = await _svc.UpdateAsync(id, req);
            return updated == null ? NotFound() : Ok(updated);
        }

        // DELETE /api/categories/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _svc.DeleteAsync(id);
            return ok ? NoContent() : NotFound();
        }
    }
}
