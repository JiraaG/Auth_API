using System.ComponentModel.DataAnnotations;

namespace Auth_API.Domain.Entities.Warehouses.Products
{
    public class OrderResponse
    {
        public int Id { get; set; }

        public string Customer { get; set; } = string.Empty;

        // Quantità e unità di misura
        public decimal Quantity { get; set; }
        [MaxLength(10)] public string QuantityUnit { get; set; } = string.Empty;

        // Tipo: true = acquisto, false = vendita
        //public bool IsPurchase { get; set; }

        // Prezzo unitario e totale
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }

        // Date di ordine e di evasione (opzionale)
        public DateTime OrderDate { get; set; }

        // Auditing
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
    }

}
