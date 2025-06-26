using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using ToDoApp.Models;

namespace ToDoApp.Services.Identity
{
    public class CustomClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>
    {
        public CustomClaimsPrincipalFactory(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<IdentityOptions> optionsAccessor) : base(userManager, roleManager, optionsAccessor)
        {

        }

        //Abchecken Hinterfragen
        // Erweiterung von ASP.NET Core Identity um benutzerdefinierte Claims zur besseren UI-Darstellung und Zugriffskontrolle.
        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
        {
            var identity = await base.GenerateClaimsAsync(user);
            var roles = await UserManager.GetRolesAsync(user);

            foreach (var role in roles)
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, role));
            }

            if (!string.IsNullOrEmpty(user.FirstName))
            {
                identity.AddClaim(new Claim("FirstName", user.FirstName));
            }
            return identity;
        }
    }
}
