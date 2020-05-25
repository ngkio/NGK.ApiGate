using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Services;

namespace Ngk.IdentityService.Common
{
    public class ProfileService : IProfileService
    {
        public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            //depending on the scope accessing the user data.
            var claims = context.Subject.Claims.ToList();
            var types = context.Client.Claims.Select(p => p.Type);
            claims.RemoveAll(p => types.Contains(p.Type));
            claims = claims.Concat(context.Client.Claims).ToList();

            //set issued claims to return
            context.IssuedClaims = claims.ToList();
            return Task.CompletedTask;
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            context.IsActive = true;
            return Task.CompletedTask;
        }
    }
}