using Auth_API.Application.Interfaces.Warehouse;
using Auth_API.Domain.Entities.Warehouses.Products;
using Auth_API.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;

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
            // Recupera il prodotto collegato
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId);
            if (product == null)
                throw new InvalidOperationException("Prodotto non trovato.");

            // Calcola la nuova quantità totale
            if (product.TotalQuantity < request.Quantity)
                throw new InvalidOperationException("Quantità richiesta superiore alla disponibilità.");

            product.TotalQuantity -= request.Quantity;

            if (product.TotalQuantity == 0)
                product.IsSold = true;

            product.SaleOrdersPrice += request.TotalPrice;

            var order = new Order
            {
                Customer = request.Customer,
                Quantity = request.Quantity,
                QuantityUnit = request.QuantityUnit,
                UnitPrice = request.UnitPrice,
                TotalPrice = request.TotalPrice,
                OrderDate = request.OrderDate,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow,
                ProductId = productId
            };

            _context.Orders.Add(order);
            _context.Products.Update(product); // Aggiorna la quantità totale
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<Order?> UpdateAsync(int id, OrderRequest request, int productId)
        {
            var existing = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == id && o.ProductId == productId);

            if (existing == null) return null;

            // Recupera il prodotto collegato
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId);
            if (product == null)
                throw new InvalidOperationException("Prodotto non trovato.");

            // Ripristina la quantità totale aggiungendo la quantità precedente dell'ordine
            product.TotalQuantity += existing.Quantity;
            product.SaleOrdersPrice -= existing.TotalPrice;

            // Controlla se la nuova quantità è disponibile
            if (product.TotalQuantity < request.Quantity)
                throw new InvalidOperationException("Quantità richiesta superiore alla disponibilità.");

            // Sottrai la nuova quantità dell'ordine
            product.TotalQuantity -= request.Quantity;

            if (product.TotalQuantity == 0)
                product.IsSold = true;

            product.SaleOrdersPrice += request.TotalPrice;

            existing.Customer = request.Customer;
            existing.Quantity = request.Quantity;
            existing.QuantityUnit = request.QuantityUnit;
            existing.UnitPrice = request.UnitPrice;
            existing.TotalPrice = request.TotalPrice;
            existing.OrderDate = request.OrderDate;
            existing.ModifiedAt = DateTime.UtcNow;

            _context.Orders.Update(existing);
            _context.Products.Update(product); // Aggiorna la quantità totale
            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id, int productId)
        {
            var existing = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == id && o.ProductId == productId);

            if (existing == null) return false;

            // Recupera il prodotto collegato
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId);
            if (product == null)
                throw new InvalidOperationException("Prodotto non trovato.");

            // Ripristina la quantità totale aggiungendo la quantità dell'ordine eliminato
            product.TotalQuantity += existing.Quantity;

            if (product.TotalQuantity > 0)
                product.IsSold = false;

            product.SaleOrdersPrice -= existing.TotalPrice;

            _context.Orders.Remove(existing);
            _context.Products.Update(product); // Aggiorna la quantità totale
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
