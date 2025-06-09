namespace Auth_API.Domain.Entities.Account.ViewModel
{
    public class RegisterConfirmationViewModel
    {
        public bool ConfirmedEmail { get; set; }

        // Contiene l'URL a cui tornare dopo il login (passato da IdentityServer)
        public string ReturnUrl { get; set; }
    }
}
