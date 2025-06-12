using Auth_API.Application.Interfaces.Warehouse;
using Auth_API.Domain.Entities.Warehouses.Products;
using Auth_API.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Auth_API.Application.Services.Warehouse
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;

        public OrderService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Order>> GetAllAsync(int productId)
        {
            return await _context.Orders
                .Where(o => o.ProductId == productId)
                .Include(o => o.Product)
                .ToListAsync();
        }

        public async Task<Order?> GetByIdAsync(int id, int productId)
        {
            return await _context.Orders
                .Where(o => o.Id == id && o.ProductId == productId)
                .Include(o => o.Product)
                .FirstOrDefaultAsync();
        }

        public async Task<Order> CreateAsync(OrderRequest request, int productId)
        {
            var order = new Order
            {
                Customer = request.Customer,
                Quantity = request.Quantity,
                QuantityUnit = request.QuantityUnit,
                UnitPrice = request.UnitPrice,
                TotalPrice = request.TotalPrice,
                OrderDate = request.OrderDate,
                FulfillmentDate = request.FulfillmentDate,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow,
                ProductId = productId
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<Order?> UpdateAsync(int id, OrderRequest request, int productId)
        {
            var existing = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == id && o.ProductId == productId);

            if (existing == null) return null;

            existing.Customer = request.Customer;
            existing.Quantity = request.Quantity;
            existing.QuantityUnit = request.QuantityUnit;
            existing.UnitPrice = request.UnitPrice;
            existing.TotalPrice = request.TotalPrice;
            existing.OrderDate = request.OrderDate;
            existing.FulfillmentDate = request.FulfillmentDate;
            existing.ModifiedAt = DateTime.UtcNow;

            _context.Orders.Update(existing);
            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id, int productId)
        {
            var existing = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == id && o.ProductId == productId);

            if (existing == null) return false;

            _context.Orders.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
