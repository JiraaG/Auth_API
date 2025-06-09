using System.ComponentModel.DataAnnotations;
using Auth_API.Domain.Entities.Enum;

namespace Auth_API.Domain.Entities.Account.ViewModel
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Il nome è obbligatorio")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Il nome deve essere lungo tra 2 e 50 caratteri")]
        [Display(Name = "Nome")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Il cognome è obbligatorio")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Il cognome deve essere lungo tra 2 e 50 caratteri")]
        [Display(Name = "Cognome")]
        public string LastName { get; set; } = string.Empty;

        [Display(Name = "Indirizzo")]
        public string? Address { get; set; }

        [Required(ErrorMessage = "L'email è obbligatoria")]
        [EmailAddress(ErrorMessage = "Formato email non valido")]
        [StringLength(100, MinimumLength = 10, ErrorMessage = "L'email deve essere lunga tra 10 e 100 caratteri")]
        // Validazione personalizzata: l'email deve terminare per .it o .com
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.(it|com)$", ErrorMessage = "L'email deve avere un provider valido che termini per .it o .com")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La password è obbligatoria")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "La password deve essere lunga almeno 8 caratteri")]
        // Validazione personalizzata: deve contenere almeno una lettera e un numero (la regex può essere adattata alle tue esigenze)
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]{8,}$",ErrorMessage = "La password deve contenere almeno una lettera e un numero")]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "La conferma password è obbligatoria")]
        [Compare("Password", ErrorMessage = "La password e la conferma non coincidono")]
        [DataType(DataType.Password)]
        [Display(Name = "Conferma Password")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Accettare i termini è obbligatorio")]
        [Display(Name = "Termini e servizi")]
        public bool AgreeTerm { get; set; }

        [Required(ErrorMessage = "Scegliere un metodo di autenticazione")]
        [Display(Name = "Metodo autenticazione a due fattori")]
        public TwoFactorMethodType TwoFactorMethod { get; set; }

        public string? ReturnUrl { get; set; }
    }
}
