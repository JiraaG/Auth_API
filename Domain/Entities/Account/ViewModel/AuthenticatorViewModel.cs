namespace Auth_API.Domain.Entities.Account.ViewModel
{
    public class AuthenticatorViewModel
    {
        public string SharedKey { get; set; }
        public string QrCodeImageUrl { get; set; }
        //public string Code { get; set; }
        public List<DigitModel> Pincode { get; set; } = Enumerable.Range(0, 6)
                                                .Select(_ => new DigitModel())
                                                .ToList();
        public string ReturnUrl { get; set; }
    }
}
