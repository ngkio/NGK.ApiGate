using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.Extensions.Configuration;

namespace Ngk.IdentityService.Common
{
    public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private IConfiguration _configuration;

        public ResourceOwnerPasswordValidator(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            var userInfo = context.Request.Raw["UInfo"];
            var userId = context.Request.Raw["UId"];

            if (context.UserName != _configuration["IdentityConfig:Auth:userName"] ||
                context.Password != _configuration["IdentityConfig:Auth:password"])
            {
                context.Result =
                    new GrantValidationResult(TokenRequestErrors.InvalidGrant, "Invalid client credential");
            }
            else
            {
                context.Result = new GrantValidationResult(
                    subject: userId,
                    authenticationMethod: "custom",
                    claims: new Claim[]
                    {
                        new Claim("baseInfo", userInfo)
                    }
                );
            }

            return Task.CompletedTask;
        }
    }
}