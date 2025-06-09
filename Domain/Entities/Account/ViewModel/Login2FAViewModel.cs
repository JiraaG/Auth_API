using System.ComponentModel.DataAnnotations;
using Auth_API.Domain.Entities.Enum;

namespace Auth_API.Domain.Entities.Account.ViewModel
{
    public class Login2FAViewModel
    {
        //[Required(ErrorMessage = "Il codice di autenticazione è obbligatorio.")]
        //[Display(Name = "Codice di autenticazione")]
        //[StringLength(7, MinimumLength = 6, ErrorMessage = "Il codice deve contenere almeno 6 caratteri.")]
        //public string TwoFactorCode { get; set; }

        //[Required(ErrorMessage = "Il codice di autenticazione è obbligatorio.")]
        //[MinLength(6, ErrorMessage = "Inserisci tutte e 6 le cifre.")]
        //[MaxLength(6, ErrorMessage = "Inserisci al massimo 6 cifre.")]
        //public string[] Pincode { get; set; } = new string[6];

        public List<DigitModel> Pincode { get; set; } = Enumerable.Range(0, 6)
                                                        .Select(_ => new DigitModel())
                                                        .ToList();

        public bool RememberMe { get; set; }

        // Facoltativo: per ricordare il dispositivo e non richiedere 2FA in futuro.
        [Display(Name = "Ricorda questo dispositivo")]
        public bool RememberMachine { get; set; }

        public TwoFactorMethodType TwoFactorMethod { get; set; }

        public string? ReturnUrl { get; set; }
    }
}
