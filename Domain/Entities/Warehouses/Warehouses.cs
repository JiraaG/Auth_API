using Auth_API.Domain.Entities.Account;

namespace Auth_API.Domain.Entities.Warehouses
{
    public class Warehouses
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreationDate { get; set; }
        public DateTime ModifiedDate { get; set; }

        // FK verso User
        public string UserId { get; set; }
        public User User { get; set; }
    }
}
