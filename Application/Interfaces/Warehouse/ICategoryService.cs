using Auth_API.Domain.Entities.Warehouses.Products;

namespace Auth_API.Application.Interfaces.Warehouse
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryResponse>> GetAllAsync();
        Task<CategoryResponse?> GetByIdAsync(int id);
        Task<CategoryResponse> CreateAsync(CategoryRequest request);
        Task<CategoryResponse?> UpdateAsync(int id, CategoryRequest request);
        Task<bool> DeleteAsync(int id);
    }
}
