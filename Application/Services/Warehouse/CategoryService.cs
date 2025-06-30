using Auth_API.Application.Interfaces.Warehouse;
using Auth_API.Domain.Entities.Warehouses.Products;
using Auth_API.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Auth_API.Application.Services.Warehouse
{
    public class CategoryService : ICategoryService
    {
        private readonly AppDbContext _ctx;

        public CategoryService(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<IEnumerable<CategoryResponse>> GetAllAsync()
        {
            var cats = await _ctx.Categories
                .Include(c => c.ParentCategory)
                .OrderBy(c => c.ParentCategoryId)  // padri prima
                .ThenBy(c => c.Name)
                .ToListAsync();

            return cats.Select(c => new CategoryResponse
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                ParentCategoryId = c.ParentCategoryId,
                ParentCategoryName = c.ParentCategory?.Name
            });
        }

        public async Task<CategoryResponse?> GetByIdAsync(int id)
        {
            var c = await _ctx.Categories
                .Include(x => x.ParentCategory)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (c == null) return null;

            return new CategoryResponse
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                ParentCategoryId = c.ParentCategoryId,
                ParentCategoryName = c.ParentCategory?.Name
            };
        }

        public async Task<CategoryResponse> CreateAsync(CategoryRequest r)
        {
            var c = new Category
            {
                Name = r.Name,
                Description = r.Description,
                ParentCategoryId = r.ParentCategoryId,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow
            };
            _ctx.Categories.Add(c);
            await _ctx.SaveChangesAsync();

            return new CategoryResponse
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                ParentCategoryId = c.ParentCategoryId,
                ParentCategoryName = (await _ctx.Categories.FindAsync(c.ParentCategoryId))?.Name
            };
        }

        public async Task<CategoryResponse?> UpdateAsync(int id, CategoryRequest r)
        {
            var c = await _ctx.Categories.FindAsync(id);
            if (c == null) return null;

            c.Name = r.Name;
            c.Description = r.Description;
            c.ParentCategoryId = r.ParentCategoryId;
            c.ModifiedAt = DateTime.UtcNow;
            await _ctx.SaveChangesAsync();

            return new CategoryResponse
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                ParentCategoryId = c.ParentCategoryId,
                ParentCategoryName = (await _ctx.Categories.FindAsync(c.ParentCategoryId))?.Name
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var c = await _ctx.Categories.FindAsync(id);
            if (c == null) return false;
            _ctx.Categories.Remove(c);
            await _ctx.SaveChangesAsync();
            return true;
        }
    }
}
