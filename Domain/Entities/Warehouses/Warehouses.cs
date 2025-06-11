using System.ComponentModel.DataAnnotations;
using Auth_API.Domain.Entities.Account;
using Auth_API.Domain.Entities.Warehouses.Products;

namespace Auth_API.Domain.Entities.Warehouses
{
    public class Warehouses
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Description { get; set; } = string.Empty;
        [Required]
        public DateTime CreationDate { get; set; }
        [Required]
        public DateTime ModifiedDate { get; set; }

        // FK verso User
        public string UserId { get; set; }
        public User User { get; set; }

        // Relazione inversa ai prodotti
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
