using System.Collections.Generic;
using System.Linq;
using IdentityServer4;
using IdentityServer4.Models;
using Microsoft.Extensions.Configuration;

namespace Ngk.IdentityService.Common
{
    public static class OAuth2Config
    {
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("NGK", "NGK API")
            };
        }

        /// <summary>
        /// Define which IdentityResources will use this IdentityServer
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.Profile()
            };
        }

        public static IEnumerable<Client> GetClients(IConfiguration configuration)
        {
            var results = new List<Client>();
            var clients = configuration.GetSection("IdentityConfig:IdentityClients");
            foreach (var client in clients.GetChildren())
            {
                results.Add(new Client
                {
                    //client_id
                    ClientId = client["ClientId"],
                    AllowedGrantTypes = client.GetSection("AllowedGrantTypes").GetChildren().Select(p => p.Value)
                        .ToList(), //Resource Owner Password模式
                    AccessTokenLifetime = int.Parse(client["AccessTokenLifetime"]), //有效期
                    ClientSecrets =
                    {
                        new Secret(client["ClientSecrets"].Sha256())
                    },
                    //scope
                    AllowedScopes = client.GetSection("AllowedScopes").GetChildren().Select(p => p.Value).ToList(),
                    AllowOfflineAccess = false,
                    UpdateAccessTokenClaimsOnRefresh = true,
                    //AccessTokenLifetime = 3600, //AccessToken的过期时间， in seconds (defaults to 3600 seconds / 1 hour)
                    //AbsoluteRefreshTokenLifetime = 60, //RefreshToken的最大过期时间，in seconds. Defaults to 2592000 seconds / 30 day
                    //RefreshTokenUsage = TokenUsage.OneTimeOnly,   //默认状态，RefreshToken只能使用一次，使用一次之后旧的就不能使用了，只能使用新的RefreshToken
                    //RefreshTokenUsage = TokenUsage.ReUse,   //可重复使用RefreshToken，RefreshToken，当然过期了就不能使用了
                });
            }

            return results;
        }
    }
}