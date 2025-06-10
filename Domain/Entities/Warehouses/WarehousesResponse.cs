using System.ComponentModel.DataAnnotations;

namespace Auth_API.Domain.Entities.Warehouses
{
    public class WarehousesResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreationDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
