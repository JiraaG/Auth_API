namespace Auth_API.Domain.Entities.Warehouses.Products
{
    public class OrderRequest
    {
        public string Customer { get; set; } = "";
        public decimal Quantity { get; set; }
        public string QuantityUnit { get; set; } = "";
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime OrderDate { get; set; }
    }
}
