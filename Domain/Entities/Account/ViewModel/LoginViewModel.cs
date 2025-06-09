using System.ComponentModel.DataAnnotations;

namespace Auth_API.Domain.Entities.Account.ViewModel
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Il campo Username è obbligatorio")]
        [Display(Name = "Username")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Il campo Password è obbligatorio")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Rimani connesso")]
        public bool RememberLogin { get; set; }

        // Contiene l'URL a cui tornare dopo il login (passato da IdentityServer)
        public string? ReturnUrl { get; set; }
    }
}
