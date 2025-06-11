using Auth_API.Domain.Entities.Warehouses;

namespace Auth_API.Application.Interfaces.Warehouse
{
    public interface IWarehouseService
    {
        Task<IEnumerable<Warehouses>> GetAllAsync(string UserId);
        Task<Warehouses?> GetByIdAsync(int id, string UserId);
        Task<Warehouses> CreateAsync(WarehousesRequest warehouse, string UserId);
        Task<Warehouses?> UpdateAsync(int id, WarehousesRequest warehouse, string UserId);
        Task<bool> DeleteAsync(int id, string UserId);
    }
}
