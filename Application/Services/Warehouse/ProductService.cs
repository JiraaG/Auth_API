using Auth_API.Application.Interfaces.Warehouse;
using Auth_API.Domain.Entities.Warehouses;
using Auth_API.Domain.Entities.Warehouses.Products;
using Auth_API.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Auth_API.Application.Services.Warehouse
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _context;

        public ProductService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllAsync(int idWarehouse)
        {
            return await _context.Products
                                    .Where(w => w.WarehouseId == idWarehouse) // filtra per UserId
                                    .Include(w => w.Warehouse) // carica anche l'utente associato
                                    .Include(w => w.Category) // carica anche la categoria associata
                                    .Include(w => w.Orders) // carica anche gli ordini associati
                                    .ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(int id, int idWarehouse)
        {
            return await _context.Products
                                    .Where(w => w.Id == id && w.WarehouseId == idWarehouse) // filtra per Id e UserId
                                    .Include(w => w.Warehouse)
                                    .Include(w => w.Category) // carica anche la categoria associata
                                    .Include(w => w.Orders) // carica anche gli ordini associati
                                    .FirstOrDefaultAsync(w => w.Id == id);
        }

        public async Task<Product> CreateAsync(ProductRequest productCreate, int idWarehouse)
        {
            Product product = new();
            product.Code = productCreate.Code;

            product.Name = productCreate.Name;
            product.Description = productCreate.Description;

            product.PurchasePrice = productCreate.PurchasePrice;
            product.PurchaseDate = productCreate.PurchaseDate;

            product.IsActive = productCreate.IsActive;
            product.IsSold = productCreate.IsSold;

            product.SalePrice = productCreate.SalePrice;
            product.SaleDate = productCreate.SaleDate;

            product.Currency = productCreate.Currency;

            product.CreatedAt = DateTime.UtcNow;
            product.ModifiedAt = DateTime.UtcNow;

            product.CategoryId = productCreate.CategoryId;
            product.WarehouseId = idWarehouse;

            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<Product?> UpdateAsync(int id, ProductRequest producteUpdate, int idWarehouse)
        {
            var existing = await _context.Products.FirstOrDefaultAsync(w => w.Id == id && w.WarehouseId == idWarehouse);
            if (existing == null) return null;

            existing.Code = producteUpdate.Code;

            existing.Name = producteUpdate.Name;
            existing.Description = producteUpdate.Description;

            existing.PurchasePrice = producteUpdate.PurchasePrice;
            existing.PurchaseDate = producteUpdate.PurchaseDate;

            existing.IsActive = producteUpdate.IsActive;
            existing.IsSold = producteUpdate.IsSold;

            existing.SalePrice = producteUpdate.SalePrice;
            existing.SaleDate = producteUpdate.SaleDate;

            existing.Currency = producteUpdate.Currency;

            existing.CreatedAt = DateTime.UtcNow;
            existing.ModifiedAt = DateTime.UtcNow;

            existing.CategoryId = producteUpdate.CategoryId;

            _context.Products.Update(existing);
            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id, int idWarehouse)
        {
            var existing = await _context.Products.FirstOrDefaultAsync(w => w.Id == id && w.WarehouseId == idWarehouse);
            if (existing == null) return false;

            _context.Products.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
