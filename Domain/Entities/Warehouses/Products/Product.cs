using System.ComponentModel.DataAnnotations;

namespace Auth_API.Domain.Entities.Warehouses.Products
{
    public class Product
    {
        public int Id { get; set; }

        // Codice univoco (SKU, Barcode, ecc.)
        public string? Code { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Description { get; set; } = string.Empty;

        // Quantità e unità di misura
        [Required]
        public decimal Quantity { get; set; }
        [Required]
        [MaxLength(10)] public string QuantityUnit { get; set; } = string.Empty;
        [Required]
        public decimal TotalQuantity { get; set; }

        // Prezzo e data di acquisto (sempre obbligatori)
        [Required]
        public decimal PurchasePrice { get; set; }
        [Required]
        public DateTime PurchaseDate { get; set; }

        // Flag di vendita: true se effettivamente venduto
        [Required]
        public bool IsSold { get; set; } = false;

        // Prezzo e data di vendita (valori significativi solo se IsSold == true)
        [Required]
        public decimal SaleOrdersPrice { get; set; }

        // Valuta ISO (EUR, USD, ecc.)
        [Required]
        public string Currency { get; set; } = "EUR";

        // Date di auditing
        [Required]
        public DateTime CreatedAt { get; set; }
        [Required]
        public DateTime ModifiedAt { get; set; }

        // FK verso Categoria
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        // FK verso Magazzino
        public int WarehouseId { get; set; }
        public Warehouses Warehouse { get; set; } = null!;

        // Ordini associati
        public ICollection<Order> Orders { get; set; }
            = new List<Order>();
    }
}
