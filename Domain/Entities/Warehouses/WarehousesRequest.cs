using System.ComponentModel.DataAnnotations;

namespace Auth_API.Domain.Entities.Warehouses
{
    public class WarehousesRequest
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Description { get; set; } = string.Empty;
        [Required]
        public DateTime CreationDate { get; set; }
        [Required]
        public DateTime ModifiedDate { get; set; }
    }
}
