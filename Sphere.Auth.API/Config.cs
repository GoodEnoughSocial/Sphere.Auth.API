using Duende.IdentityServer;
using Duende.IdentityServer.Models;

using IdentityModel;

using Sphere.Shared;

namespace Sphere.Auth.API;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResource
            {
                Name = "verification",
                UserClaims = new List<string>
                {
                    JwtClaimTypes.Email,
                    JwtClaimTypes.EmailVerified,
                }
            }
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new ApiScope[]
        {
            new ApiScope(name: "api1", displayName: "MyAPI"),
        };

    public static IEnumerable<Client> Clients =>
        new Client[]
        {
            new Client
            {
                ClientId = "client",
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets =
                {
                    new Secret("secret".Sha256()),
                },
                AllowedScopes =
                {
                    "api1",
                }
            },
            new Client
            {
                ClientId = "web",
                ClientSecrets =
                {
                   new Secret("secret".Sha256()),
                },
                AllowedGrantTypes = GrantTypes.Code,
                // where to redirect to after login
                // RedirectUris = { Services.SphereWebTestApp.Combine("signin-oidc") },

                AllowOfflineAccess = true,

                // where to redirect to after logout
                // PostLogoutRedirectUris = { Services.SphereWebTestApp.Combine("signout-callback-oidc") },

                AllowedScopes = new List<string>
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    "api1",
                }
            },
        };
}
