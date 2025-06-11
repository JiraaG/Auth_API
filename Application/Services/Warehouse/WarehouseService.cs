using Auth_API.Application.Interfaces.Warehouse;
using Auth_API.Domain.Entities.Warehouses;
using Auth_API.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Auth_API.Application.Services.Warehouse
{
    public class WarehouseService : IWarehouseService
    {
        private readonly AppDbContext _context;

        public WarehouseService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Warehouses>> GetAllAsync(string UserId)
        {
            return await _context.Warehouses
                                    .Where(w => w.UserId == UserId) // filtra per UserId
                                    .Include(w => w.User) // carica anche l'utente associato
                                    .ToListAsync();
        }

        public async Task<Warehouses?> GetByIdAsync(int id, string UserId)
        {
            return await _context.Warehouses
                                    .Where(w => w.Id == id && w.UserId == UserId) // filtra per Id e UserId
                                    .Include(w => w.User)
                                    .FirstOrDefaultAsync(w => w.Id == id);
        }

        public async Task<Warehouses> CreateAsync(WarehousesRequest warehouseCreate, string UserId)
        {

            Warehouses warehouse = new();
            warehouse.Name = warehouseCreate.Name;
            warehouse.Description = warehouseCreate.Description;
            warehouse.CreationDate = DateTime.UtcNow;
            warehouse.ModifiedDate = warehouseCreate.CreationDate;
            warehouse.UserId = UserId;

            _context.Warehouses.Add(warehouse);
            await _context.SaveChangesAsync();
            return warehouse;
        }

        public async Task<Warehouses?> UpdateAsync(int id, WarehousesRequest warehouseUpdate, string UserId)
        {
            var existing = await _context.Warehouses.FirstOrDefaultAsync(w => w.Id == id && w.UserId == UserId);
            if (existing == null) return null;

            existing.Name = warehouseUpdate.Name;
            existing.Description = warehouseUpdate.Description;
            existing.ModifiedDate = DateTime.UtcNow;

            _context.Warehouses.Update(existing);
            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id, string UserId)
        {
            var existing = await _context.Warehouses.FirstOrDefaultAsync(w => w.Id == id && w.UserId == UserId);
            if (existing == null) return false;

            _context.Warehouses.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
