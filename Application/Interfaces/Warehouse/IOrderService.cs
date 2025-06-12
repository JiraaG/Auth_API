using Auth_API.Domain.Entities.Warehouses.Products;

namespace Auth_API.Application.Interfaces.Warehouse
{
    public interface IOrderService
    {
        Task<IEnumerable<Order>> GetAllAsync(int productId);
        Task<Order?> GetByIdAsync(int id, int productId);
        Task<Order> CreateAsync(OrderRequest request, int productId);
        Task<Order?> UpdateAsync(int id, OrderRequest request, int productId);
        Task<bool> DeleteAsync(int id, int productId);
    }
}
