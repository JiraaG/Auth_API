using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Auth_API.Application.Services;
using Auth_API.Domain.Entities.Account;
using Auth_API.Domain.Entities.Account.ViewModel;
using Auth_API.Domain.Entities.Enum;
using Auth_API.Views.Account;
using Duende.IdentityModel;
using Duende.IdentityServer;
using Duende.IdentityServer.Events;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR.Protocol;
using QRCoder;

namespace Auth_API.Controllers.Account
{
    [Route("account")]
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        private readonly EmailCustomSender _emailSender;

        private readonly IIdentityServerInteractionService _interaction;
        private readonly IEventService _events;

        private readonly UrlEncoder _urlEncoder;

        public AccountController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            EmailCustomSender emailSender,
            IIdentityServerInteractionService interaction,
            IEventService events,
            UrlEncoder urlEncoder)
        {
            _userManager = userManager;
            _signInManager = signInManager;

            _emailSender = emailSender;

            _interaction = interaction;
            _events = events;
            _urlEncoder = urlEncoder;
        }

        [HttpGet("login")]
        public async Task<IActionResult> Login(string returnUrl)
        {
            var vm = new LoginViewModel { ReturnUrl = returnUrl };
            return View(vm);
        }

        [HttpPost("login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // 1) Recupera il contesto di autorizzazione (può essere null)
            //var context = await _interaction.GetAuthorizationContextAsync(model.ReturnUrl);

            // 2) Verifica le credenziali con Identity
            var result = await _signInManager.PasswordSignInAsync(
                model.Email,
                model.Password,
                model.RememberLogin,
                lockoutOnFailure: true);

            if (result.IsLockedOut)
            {
                // Gestisci il caso di utente bloccato
                return View("Lockout");
            }
            else if (result.RequiresTwoFactor)
            {
                // L'utente ha abilitato il 2FA. Recupera l'utente per generare il token.
                var userValue = await _userManager.FindByEmailAsync(model.Email);

                // Se l'utente ha scelto il metodo "Email", genera il token e invia la mail.
                if (userValue.TwoFactorEnabled && userValue.TwoFactorMethod == TwoFactorMethodType.Email)
                {
                    var token = await _userManager.GenerateTwoFactorTokenAsync(userValue, "Email");

                    if (string.IsNullOrEmpty(token))
                    {
                        ModelState.AddModelError(string.Empty, "Errore nella generazione del token per l'autenticazione");
                        return View(model);
                    }

                    await _emailSender.SendEmailAsync(
                        userValue.Email,
                        "Codice di Autenticazione",
                        $"Il tuo codice di autenticazione è: <strong>{token}</strong>");
                }

                // Reindirizza alla pagina per l'inserimento del codice 2FA.
                return RedirectToAction("Login2FA", "Account", new
                {
                    rememberMe = model.RememberLogin,
                    returnUrl = model.ReturnUrl,
                    twoFactorMethod = userValue.TwoFactorMethod
                });
            }
            else if (!result.Succeeded)
            {
                // Se non ha successo e non è un caso 2FA, le credenziali non sono valide.
                ModelState.AddModelError(string.Empty, "Username o password non validi");
                return View(model);
            }

            return View("Error");

            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Username o password non validi");
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user.TwoFactorEnabled && user.TwoFactorMethod == TwoFactorMethodType.Email)
            {
                var token = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");

                if (token == null)
                {
                    ModelState.AddModelError(string.Empty, "Errore nella generazione del token per l'autenticazione");
                    return View(model);
                }

                await _emailSender.SendEmailAsync(
                    user.Email,
                    "Codice di Autenticazione",
                    $"Il tuo codice di autenticazione è: <strong>{token}</strong>");
            }

            // autenticazione a 2 fattori
            // Renderizzo a Login2FA se autenticato tramite email e password
            return RedirectToAction("Login2FA", "Account", new { rememberMe = model.RememberLogin, returnUrl = model.ReturnUrl });

            //// 3) Hai validato l’utente, alza l’evento di login
            //var user = await _userManager.FindByEmailAsync(model.Email);
            //await _events.RaiseAsync(new UserLoginSuccessEvent(
            //    model.Email,
            //    user.Id,
            //    model.Email,
            //    clientId: context?.Client.ClientId));

            //// 4) Genera qui il cookie di IdentityServer (idsrv)
            ////    In questo modo la sessione viene “vista” da /connect/authorize
            //var props = new AuthenticationProperties
            //{
            //    IsPersistent = model.RememberLogin,
            //    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1)
            //};
            //// crea il ClaimsPrincipal con tutti i claims (inclusi quelli di IdentityServer)
            //var principal = await _signInManager.CreateUserPrincipalAsync(user);

            //// -----------------------------
            //// cast ad identity per aggiungere il claim “idp”
            //if (principal.Identity is ClaimsIdentity id)
            //{
            //    // JwtClaimTypes.IdentityProvider = "idp"
            //    id.AddClaim(new Claim(JwtClaimTypes.IdentityProvider, IdentityServerConstants.LocalIdentityProvider));
            //}
            //// -----------------------------

            //// e firma sullo schema corretto
            //await HttpContext.SignInAsync(
            //    IdentityServerConstants.DefaultCookieAuthenticationScheme,
            //    principal,
            //    props);

            //// 5) Infine redirect al returnUrl (che è /connect/authorize/callback?…)
            //if (context != null)
            //{
            //    return LocalRedirect(model.ReturnUrl);
            //}

            //return RedirectToAction("Index", "Home");
        }

        [HttpGet("Login2FA")]
        public async Task<IActionResult> Login2FA(bool rememberMe, string returnUrl, TwoFactorMethodType twoFactorMethod)
        {
            // Recupera l'utente attivo in autenticazione 2FA
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new InvalidOperationException("Impossibile caricare l'utente per l'autenticazione a due fattori.");
            }

            var model = new Login2FAViewModel
            {
                RememberMe = rememberMe,
                ReturnUrl = returnUrl,
                TwoFactorMethod = twoFactorMethod // es. "Email" o "Authenticator"
            };
            return View(model);
        }

        [HttpPost("Login2FA")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login2FA(Login2FAViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Recupera l'utente in attesa dell'autenticazione a due fattori.
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new InvalidOperationException("Impossibile trovare l'utente per l'autenticazione a due fattori.");
            }

            // Concateno il codice inserito
            var code = string.Concat(model.Pincode.Select(d => d.Value));

            // Esegui il sign-in in base al provider
            Microsoft.AspNetCore.Identity.SignInResult signInResultValue;
            switch (user.TwoFactorMethod)
            {
                case TwoFactorMethodType.Email:
                    // Verifica il token via UserManager, poi sign-in
                    var validEmailToken = await _userManager.VerifyTwoFactorTokenAsync(user, TokenOptions.DefaultEmailProvider, code);
                    if (!validEmailToken)
                    {
                        ModelState.AddModelError(string.Empty, "Il codice di autenticazione non è valido.");
                        return View(model);
                    }
                    await _signInManager.SignInAsync(user, model.RememberMe);
                    signInResultValue = Microsoft.AspNetCore.Identity.SignInResult.Success;
                    break;

                case TwoFactorMethodType.Authenticator:
                    signInResultValue = await _signInManager.TwoFactorSignInAsync(
                        TokenOptions.DefaultAuthenticatorProvider,
                        code,
                        model.RememberMe,
                        model.RememberMachine);
                    if (!signInResultValue.Succeeded)
                    {
                        ModelState.AddModelError(string.Empty, "Il codice di autenticazione non è valido.");
                        return View(model);
                    }
                    break;

                default:
                    ModelState.AddModelError(string.Empty, "Metodo di autenticazione non supportato.");
                    return View(model);
            }

            // Alza l'evento di successo
            var contextValue = await _interaction.GetAuthorizationContextAsync(model.ReturnUrl);
            await _events.RaiseAsync(new UserLoginSuccessEvent(
                user.Email,
                user.Id,
                user.Email,
                clientId: contextValue?.Client.ClientId));

            // Redirect al returnUrl o home
            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            {
                return LocalRedirect(model.ReturnUrl);
            }

            return RedirectToAction("Index", "Home");

            //bool isTokenValid = false;

            //// Controlla quale provider usare in base al metodo scelto dall'utente (salvato nel record o passato tramite model)
            //var provider = user.TwoFactorMethod; 
            //if (provider == TwoFactorMethodType.Email)
            //{
            //    // Verifica il token usando il provider "Email"
            //    isTokenValid = await _userManager.VerifyTwoFactorTokenAsync(user, "Email", model.TwoFactorCode);
            //}
            //else if (provider == TwoFactorMethodType.Authenticator)
            //{
            //    // Verifica il token utilizzando il flusso di TwoFactorSignInAsync
            //    var signInResult = await _signInManager.TwoFactorSignInAsync("Authenticator", model.TwoFactorCode, model.RememberMe, model.RememberMachine);
            //    isTokenValid = signInResult.Succeeded;
            //}
            //else
            //{
            //    ModelState.AddModelError(string.Empty, "Metodo di autenticazione non supportato.");
            //    return View(model);
            //}

            //if (!isTokenValid)
            //{
            //    ModelState.AddModelError(string.Empty, "Il codice di autenticazione non è valido.");
            //    return View(model);
            //}

            //var context = await _interaction.GetAuthorizationContextAsync(model.ReturnUrl);

            ////var user = await _userManager.FindByEmailAsync(model.Email);
            //await _events.RaiseAsync(new UserLoginSuccessEvent(
            //    user.Email,
            //    user.Id,
            //    user.Email,
            //    clientId: context?.Client.ClientId));

            //// Una volta verificato il token, procedi al login completo.
            //var principal = await _signInManager.CreateUserPrincipalAsync(user);
            //if (principal.Identity is ClaimsIdentity id)
            //{
            //    id.AddClaim(new Claim(JwtClaimTypes.IdentityProvider, IdentityServerConstants.LocalIdentityProvider));
            //    if (!principal.HasClaim(c => c.Type == JwtClaimTypes.Subject))
            //    {
            //        id.AddClaim(new Claim(JwtClaimTypes.Subject, user.Id));
            //    }
            //}

            //var props = new AuthenticationProperties
            //{
            //    IsPersistent = model.RememberMe,
            //    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1)
            //};

            //await HttpContext.SignInAsync(IdentityServerConstants.DefaultCookieAuthenticationScheme, principal, props);

            //if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            //{
            //    return LocalRedirect(model.ReturnUrl);
            //}

            //return RedirectToAction("Index", "Home");

        }

        [HttpGet("register")]
        public IActionResult Register(string returnUrl)
        {
            var vm = new RegisterViewModel { ReturnUrl = returnUrl };
            return View(vm);
        }

        [HttpPost("register")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = new User
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Address = model.Address,
                AgreeTerm = model.AgreeTerm,
                TwoFactorEnabled = true,
                TwoFactorMethod = model.TwoFactorMethod,
                CreatedAt = DateTime.UtcNow,
                PseudonymizedUserId = Guid.NewGuid(),
                EmailConfirmed = true,
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            // Prima di eseguire l'invio per la conferma dell'email, eseguire il collegamento con app authenticator
            if (result.Succeeded && user.TwoFactorMethod == TwoFactorMethodType.Authenticator)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);

                // Genera chiave e QR per il model
                var key = await _userManager.GetAuthenticatorKeyAsync(user);
                if (string.IsNullOrEmpty(key))
                {
                    await _userManager.ResetAuthenticatorKeyAsync(user);
                    key = await _userManager.GetAuthenticatorKeyAsync(user);
                }

                var issuer = UrlEncoder.Default.Encode("Manage");
                var email = UrlEncoder.Default.Encode(user.Email);
                var otpUri = $"otpauth://totp/{issuer}:{email}?secret={key}&issuer={issuer}&digits=6";

                using var qrGenerator = new QRCodeGenerator();
                using var qrData = qrGenerator.CreateQrCode(otpUri, QRCodeGenerator.ECCLevel.Q);
                using var qrCode = new PngByteQRCode(qrData);
                var qrBase64 = Convert.ToBase64String(qrCode.GetGraphic(20));

                var authenticatorModel = new AuthenticatorViewModel
                {
                    SharedKey = FormatKey(key),
                    QrCodeImageUrl = $"data:image/png;base64,{qrBase64}",
                    ReturnUrl = model.ReturnUrl
                };

                // Visualizza view di associazione Authenticator
                return View("AuthenticatorTwoFactor", authenticatorModel);
            }
            // Successivamente eseguire il confirmation email
            // Se solo succeeded invio solo il confirm email
            else if (result.Succeeded)
            {
                // Genera il token di conferma email
                var emailToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                // Genera il link di conferma (assicurati di passare il token e lo userId)
                var confirmLink = Url.Action("ConfirmEmail", "Account",
                    new { userId = user.Id, token = emailToken, returnUrl = model.ReturnUrl }, Request.Scheme);

                // Invia l'email di conferma (ogni provider di email deve essere configurato nel tuo _emailSender)
                await _emailSender.SendEmailAsync(
                    user.Email,
                    "Conferma la tua email",
                    $"Clicca sul link per confermare la tua email: <a href='{confirmLink}'>Conferma Email</a>");
            }

            // Puoi reindirizzare ad una view che spiega che è necessario confermare l'email
            return RedirectToAction("RegisterConfirmation", "Account", new { returnUrl = model.ReturnUrl, succeeded = result.Succeeded });

            //if (result.Succeeded)
            //{
            //    await _events.RaiseAsync(new UserLoginSuccessEvent(
            //        user.UserName,
            //        user.Id,
            //        user.UserName));

            //    // Non loggare direttamente l'utente dopo la registrazione, reindirizza al login
            //    return RedirectToAction("Login", "Account", new { returnUrl = model.ReturnUrl });
            //}

            //foreach (var e in result.Errors)
            //    ModelState.AddModelError(string.Empty, e.Description);

            //return View(model);
        }

        [HttpGet("AuthenticatorTwoFactor")]
        public IActionResult AuthenticatorTwoFactor(AuthenticatorViewModel authenticatorModel)
        {
            return View(authenticatorModel);
        }

        [HttpPost("Authenticator2FA")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Authenticator2FA(AuthenticatorViewModel model)
        {
            if (!ModelState.IsValid)
                return View("AuthenticatorTwoFactor");

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login");

            // Concateno il codice inserito
            var code = string.Concat(model.Pincode.Select(d => d.Value));

            var token = code.Replace(" ", string.Empty);
            var isValid = await _userManager.VerifyTwoFactorTokenAsync(
                user, _userManager.Options.Tokens.AuthenticatorTokenProvider, token);

            if (!isValid)
            {
                ModelState.AddModelError(nameof(code), "Codice non valido.");
                return View(model);
            }

            await _userManager.SetTwoFactorEnabledAsync(user, true);

            // Genera il token di conferma email
            var emailToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            // Genera il link di conferma (assicurati di passare il token e lo userId)
            var confirmLink = Url.Action("ConfirmEmail", "Account",
                new { userId = user.Id, token = emailToken, returnUrl = model.ReturnUrl }, Request.Scheme);

            // Invia l'email di conferma (ogni provider di email deve essere configurato nel tuo _emailSender)
            await _emailSender.SendEmailAsync(
                user.Email,
                "Conferma la tua email",
                $"Clicca sul link per confermare la tua email: <a href='{confirmLink}'>Conferma Email</a>");

            return RedirectToAction("RegisterConfirmation", "Account", new { returnUrl = model.ReturnUrl, succeeded = true });
        }

        [HttpGet("registerConfirmation")]
        public IActionResult RegisterConfirmation(string returnUrl, bool succeeded)
        {
            var vm = new RegisterConfirmationViewModel { ReturnUrl = returnUrl, ConfirmedEmail = succeeded };
            return View(vm);
        }

        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token, string returnUrl)
        {
            if (userId == null || token == null)
            {
                //return RedirectToAction("Index", "Home");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                //return NotFound($"Impossibile trovare l'utente con ID '{userId}'.");
            }

            //var result = await _userManager.ConfirmEmailAsync(user, token);
            var result = false;
            // Inizializza il model per la view di conferma
            var registerConfirmModel = new RegisterConfirmationViewModel
            {
                ReturnUrl = returnUrl,  // Puoi impostare il returnUrl se necessario
                // Puoi impostare il returnUrl se necessario
                ConfirmedEmail = result
            };

            return View("ConfirmEmail", registerConfirmModel);
        }

        [HttpGet("logout")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(string logoutId)
        {
            // rimuove il cookie di ASP.NET Identity
            await _signInManager.SignOutAsync();

            var context = await _interaction.GetLogoutContextAsync(logoutId);
            return Redirect(context?.PostLogoutRedirectUri ?? "/");
        }

        [HttpGet("error")]
        public async Task<IActionResult> Error(string errorId)
        {
            var vm = new ErrorViewModel();

            if (!string.IsNullOrEmpty(errorId))
            {
                var message = await _interaction.GetErrorContextAsync(errorId);
                if (message != null)
                {
                    vm.Error = message;
                }
            }

            return View(vm);
        }

        private string FormatKey(string key)
        {
            const int chunk = 4;
            var sb = new StringBuilder();
            for (int i = 0; i < key.Length; i += chunk)
                sb.Append(key.Substring(i, Math.Min(chunk, key.Length - i))).Append(' ');
            return sb.ToString().Trim().ToLowerInvariant();
        }
    }

}
