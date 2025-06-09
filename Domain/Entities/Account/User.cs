using Auth_API.Domain.Entities.Enum;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Auth_API.Domain.Entities.Account
{
    public class User : IdentityUser
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        public string? Address { get; set; }

        public bool AgreeTerm { get; set; }
        public TwoFactorMethodType TwoFactorMethod { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? LastModified { get; set; }

        [Required]
        public Guid PseudonymizedUserId { get; set; }

    }
}
