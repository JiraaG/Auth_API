using System.ComponentModel.DataAnnotations;

namespace Auth_API.Domain.Entities.Account.ViewModel
{
    public class DigitModel
    {
        [Required(ErrorMessage = "Ogni casella deve contenere un numero.")]
        [RegularExpression(@"\d", ErrorMessage = "Inserisci solo cifre.")]
        public string Value { get; set; }
    }
}
