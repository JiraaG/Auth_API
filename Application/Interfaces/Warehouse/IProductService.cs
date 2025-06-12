using Auth_API.Domain.Entities.Warehouses.Products;

namespace Auth_API.Application.Interfaces.Warehouse
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllAsync(int idWarehouse);
        Task<Product?> GetByIdAsync(int id, int idWarehouse);
        Task<Product> CreateAsync(ProductRequest productCreate, int idWarehouse);
        Task<Product?> UpdateAsync(int id, ProductRequest productUpdate, int idWarehouse);
        Task<bool> DeleteAsync(int id, int idWarehouse);
    }
}
