using IdentityServer4;
using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Khadoos.IDP
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> Ids =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Address(),
                new IdentityResource(
                    "roles",
                    "Your roles",
                    new List<string>{"role"}),
                new IdentityResource(
                    "age",
                    "how old are you",
                    new List<string>{"age"})
            };

        public static IEnumerable<ApiResource> Apis =>
           new ApiResource[]
           {
                new ApiResource("courselibraryapi", "Course Library API", new List<string>{ "role" })
           };

        public static IEnumerable<Client> Clients =>
           new Client[]
           {
                new Client
                {
                    ClientName ="Globomantics",
                    ClientId ="globomanticsclient",
                    AllowedGrantTypes =GrantTypes.Code,
                    RedirectUris = new List<string>()
                    {
                        "http://localhost:55680/signin-oidc"
                    },
                    PostLogoutRedirectUris = new List<string>()
                    {
                        "http://localhost:55680/signout-callback-oidc"
                    },
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Address,
                        "roles","courselibraryapi",
                        "age"
                    },
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    }
                },
                new Client
                {
                    //RefreshTokenExpiration = TokenExpiration.Sliding,
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientId ="courselibraryclient",
                    ClientName="Course Library Client",
                    ClientSecrets =
                    {
                        new Secret("courselibraryclient".Sha256())
                    },
                    AllowedScopes= { "courselibraryapi" }
                },
                new Client
                {
                    ClientId = "reactclient",
                    ClientName = "React Client",
                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    RequireClientSecret = false,
                    
                    RedirectUris =           { "http://localhost:3000/callback"},
                    PostLogoutRedirectUris = { "http://localhost:3000" },
                    AllowedCorsOrigins =     { "http://localhost:3000" },

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "courselibraryapi","roles"

                    }
                }
           };
    }
}
