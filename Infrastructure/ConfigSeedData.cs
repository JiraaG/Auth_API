using Duende.IdentityServer.Models;

namespace AuthGDPR.Infrastructure
{
    public static class ConfigSeedData
    {
        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId                    = "swagger-ui",
                    ClientName                  = "Swagger UI",
                    AllowedGrantTypes           = GrantTypes.Code,
                    RequirePkce                 = true,
                    RequireClientSecret         = false,
                    RedirectUris                = { "https://localhost:7009/swagger/oauth2-redirect.html" },
                    AllowedCorsOrigins          = { "https://localhost:7009" },
                    PostLogoutRedirectUris      = { "https://localhost:7009/swagger/index.html" },
                    AllowedScopes               = { "openid", "profile", "api1", "offline_access" },
                    AllowOfflineAccess          = true,
                    AllowAccessTokensViaBrowser = true
                },
                new Client
                {
                    ClientId                    = "angular-client",
                    ClientName                  = "Angular SPA Client",
                    AllowedGrantTypes           = GrantTypes.Code,
                    RequirePkce                 = true,
                    RequireClientSecret         = false,
                    RedirectUris                = { "https://localhost:4200/auth-callback" },
                    AllowedCorsOrigins          = { "https://localhost:4200" },
                    PostLogoutRedirectUris      = { "https://localhost:4200/" },
                    AllowedScopes               = { "openid", "profile", "api1", "offline_access" },
                    AllowOfflineAccess          = true,
                    AllowAccessTokensViaBrowser = true,
                    RefreshTokenUsage           = TokenUsage.ReUse,
                    RefreshTokenExpiration      = TokenExpiration.Sliding,
                    AccessTokenLifetime         = 3600,
                    SlidingRefreshTokenLifetime = 1296000,
                },
                new Client
                {
                    ClientId                    = "angular-client-http",
                    ClientName                  = "Angular SPA Client",
                    AllowedGrantTypes           = GrantTypes.Code,
                    RequirePkce                 = true,
                    RequireClientSecret         = false,
                    RedirectUris                = { "http://localhost:4200/auth-callback" },
                    AllowedCorsOrigins          = { "http://localhost:4200" },
                    PostLogoutRedirectUris      = { "http://localhost:4200/" },
                    AllowedScopes               = { "openid", "profile", "api1", "offline_access" },
                    AllowOfflineAccess          = true,
                    AllowAccessTokensViaBrowser = true,
                    RefreshTokenUsage           = TokenUsage.ReUse,
                    RefreshTokenExpiration      = TokenExpiration.Sliding,
                    AccessTokenLifetime         = 3600,
                    SlidingRefreshTokenLifetime = 1296000,
                }
            };
        }

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
                // Aggiungi altre risorse d’identità se necessario
            };
        }

        public static IEnumerable<ApiScope> GetApiScopes()
        {
            return new List<ApiScope>
            {
                new ApiScope("api1", "Accesso a My API")
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("api1", "My API")
                {
                    Scopes = { "api1" }
                }
            };
        }

    }
}
