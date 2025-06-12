using System.ComponentModel.DataAnnotations;

namespace Auth_API.Domain.Entities.Warehouses.Products
{
    public class ProductResponse
    {
        public int Id { get; set; }

        public string? Code { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        // Prezzo e data di acquisto (sempre obbligatori)
        public decimal PurchasePrice { get; set; }
        public DateTime PurchaseDate { get; set; }

        // Attributo che definisce se il prodotto è vendibile
        // true => vendibile (attivo)
        // false => non vendibile (disabilitato in UI)
        public bool IsActive { get; set; } = true;

        // Flag di vendita: true se effettivamente venduto
        public bool IsSold { get; set; } = false;

        // Prezzo e data di vendita (valori significativi solo se IsSold == true)
        public decimal SalePrice { get; set; } = 0m;
        public DateTime SaleDate { get; set; } = DateTime.MinValue;

        // Valuta ISO (EUR, USD, ecc.)
        public string Currency { get; set; } = "EUR";

        // Date di auditing
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }

        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;

        public IEnumerable<OrderResponse> Orders { get; set; } = new List<OrderResponse>();
    }
}
