using System.ComponentModel.DataAnnotations;

namespace Auth_API.Domain.Entities.Warehouses.Products
{
    public class CategoryRequest
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        // Self-reference opzionale (id della categoria padre)
        public int? ParentCategoryId { get; set; }
    }
}
