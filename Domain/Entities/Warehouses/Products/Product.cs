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

        // Prezzo e data di acquisto (sempre obbligatori)
        [Required]
        public decimal PurchasePrice { get; set; }
        [Required]
        public DateTime PurchaseDate { get; set; }

        // Attributo che definisce se il prodotto è vendibile
        // true => vendibile (attivo)
        // false => non vendibile (disabilitato in UI)
        [Required]
        public bool IsActive { get; set; } = true;

        // Flag di vendita: true se effettivamente venduto
        [Required]
        public bool IsSold { get; set; } = false;

        // Prezzo e data di vendita (valori significativi solo se IsSold == true)
        [Required]
        public decimal SalePrice { get; set; } = 0m;
        public DateTime SaleDate { get; set; } = DateTime.MinValue;

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
    }
}
