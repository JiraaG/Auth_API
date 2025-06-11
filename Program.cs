using Auth_API.Application.Interfaces.Warehouse;
using Auth_API.Application.Services;
using Auth_API.Application.Services.Warehouse;
using Auth_API.Domain.Entities.Account;
using Auth_API.Infrastructure.Persistance;
using Duende.IdentityServer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
// Imposta il percorso base e carica sempre il file specificato
builder.Configuration
       .SetBasePath(Directory.GetCurrentDirectory())
       .AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true)
       .AddEnvironmentVariables();          // Per eventuali variabili d'ambiente

var migrationsAssembly = typeof(Program).Assembly.GetName().Name;
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 1) DbContexts
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseNpgsql(connectionString));

builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    // Richiedi la conferma dell'email prima del login
    options.SignIn.RequireConfirmedEmail = true;

    // Configurazione della complessità della password, lockout, ecc.
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 8;

    // Specifica il provider per la generazione del token di conferma email
    options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;

    // Altre impostazioni (lockout, ecc.) se necessario.
})
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// -------------------- NUOVO
builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
{
    // Imposta la durata di validità dei token (ad esempio 5 minuti)
    options.TokenLifespan = TimeSpan.FromMinutes(5);
});
// ----------------------

// Add services to the container.
builder.Services.AddIdentityServer(options =>
{
    // (Opzionale) Configurazioni globali, ad esempio i tempi di scadenza dei token.
    options.UserInteraction = new Duende.IdentityServer.Configuration.UserInteractionOptions
    {
        LoginUrl = "/account/login",
        LogoutUrl = "/account/logout",
        ConsentUrl = "/account/consent",
        ErrorUrl = "/account/error",

        // Fai in modo che IdentityServer si aspetti sempre "returnUrl"
        LoginReturnUrlParameter = "returnUrl",
        ConsentReturnUrlParameter = "returnUrl",
        ErrorIdParameter = "errorId"
    };

    // Attiva i vari eventi per facilitare il debug
    options.Events.RaiseErrorEvents = true;
    options.Events.RaiseInformationEvents = true;
    options.Events.RaiseFailureEvents = true;
    options.Events.RaiseSuccessEvents = true;

    // Qui dico a Duende di aspettarsi il cookie di ASP.NET Identity
    options.Authentication.CookieAuthenticationScheme = IdentityConstants.ApplicationScheme;

})
.AddAspNetIdentity<User>()
.AddConfigurationStore(options =>
{
    // Configura il DbContext per il database di configurazione (client, risorse, API scopes, ecc.)
    options.ConfigureDbContext = b => b.UseNpgsql(connectionString,
        npgsqlOptions =>
        {
            npgsqlOptions.MigrationsAssembly(migrationsAssembly);
        });
})
.AddOperationalStore(options =>
{
    // Configura il DbContext per il database operativo (persisted grants, token, codici di autorizzazione, refresh token, ecc.)
    options.ConfigureDbContext = b => b.UseNpgsql(connectionString,
        npgsqlOptions =>
        {
            npgsqlOptions.MigrationsAssembly(migrationsAssembly);
        });
    options.EnableTokenCleanup = true;    // Abilita la pulizia automatica dei token scaduti
    options.TokenCleanupInterval = 3600;    // Intervallo in secondi per la pulizia (es. ogni ora)
})
.AddDeveloperSigningCredential();   // in sviluppo va bene, in prod .AddSigningCredential(...)

//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddJwtBearer(options =>
//    {
//        options.Authority = "https://localhost:7009"; // IdentityServer URL
//        options.Audience = "api1";                    // il nome dell'API registrato in IdentityServer
//        options.TokenValidationParameters.ValidateAudience = true;
//    });
//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//})
//.AddJwtBearer(options =>
//{
//    options.Authority = "https://localhost:7009";
//    options.Audience = "api1";
//    options.TokenValidationParameters.ValidateAudience = true;
//});
// 2) Authentication: cookie per IdentityServer UI + JwtBearer per API
//builder.Services.AddAuthentication(options =>
//{
//    // lasciamo il cookie "idsrv" come DefaultScheme per la UI di Duende
//    options.DefaultScheme = IdentityServerConstants.DefaultCookieAuthenticationScheme;
//    // ma usiamo “Bearer” per le challenge di [Authorize] sulle API
//    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//})
builder.Services.AddAuthentication()
// Aggiungi solo il JWT per le API:
.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
{
    options.Authority = "https://localhost:7009";
    options.Audience = "api1";
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateAudience = true
    };
});

// Configurazione personalizzata per il cookie "idsrv"
//builder.Services.Configure<CookieAuthenticationOptions>(
//    IdentityServerConstants.DefaultCookieAuthenticationScheme,
//    options =>
//    {
//        options.Cookie.SameSite = SameSiteMode.None;
//        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
//        options.Cookie.HttpOnly = true;
//    });

builder.Services.AddAuthorization(options =>
{
    options.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
        .RequireAuthenticatedUser()
        .RequireClaim("scope", "api1")
        .Build();
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("default", p =>
    {
        p.WithOrigins("https://localhost:7009", "https://localhost:4200", "http://localhost:4200")
         .AllowAnyHeader()
         .AllowAnyMethod()
         .AllowCredentials();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "My API", Version = "v1" });
    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            AuthorizationCode = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri("https://localhost:7009/connect/authorize"),
                TokenUrl = new Uri("https://localhost:7009/connect/token"),
                Scopes = new Dictionary<string, string>
                {
                    { "api1", "My API" },
                    { "openid", "OpenID" },
                    { "profile", "User profile" },
                    { "offline_access", "Offline access" }
                }
            }
        }
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme { Reference = new OpenApiReference
                { Type = ReferenceType.SecurityScheme, Id = "oauth2" } },
            new[] { "api1", "openid", "profile", "offline_access" }
        }
    });
});

/*
 AddTransient crea una nuova istanza ogni volta che il servizio viene iniettato. 
    Questo è ideale per servizi leggeri e stateless, come un EmailSender, 
    perché ogni operazione è indipendente e non è necessario condividere dati tra diverse richieste.
 */
builder.Services.AddTransient<EmailCustomSender>();

builder.Services.AddScoped<IWarehouseService, WarehouseService>();

builder.Services.AddControllers();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");

    // Configurazione OAuth2 per Swagger UI:
    c.OAuthClientId("swagger-ui");
    c.OAuthAppName("Swagger UI");
    c.OAuthUsePkce(); // Se stai usando il flow con PKCE
    // Se necessario, puoi aggiungere altre impostazioni come:
    // c.OAuthScopeSeparator(" ");
    // c.OAuthAdditionalQueryStringParams(new { ... });
});

// Seeding dei dati IdentityServer (Clients, ApiResources, ApiScopes, IdentityResources)
using (var scope = app.Services.CreateScope())
{
    var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
    var logger = loggerFactory.CreateLogger("SeedData");
    var env = scope.ServiceProvider.GetRequiredService<IHostEnvironment>();
    await AuthGDPR.Infrastructure.SeedData.EnsureSeedDataAsync(app, logger, env);
    logger.LogInformation("Seed completato");
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseRouting();

app.UseCors("default");

app.UseIdentityServer();

app.UseAuthentication();
app.UseAuthorization();

//// o, per garantire sia gli attribute sia la convenzione:
//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();       // importante
app.MapDefaultControllerRoute();
app.MapControllers();

app.Run();
