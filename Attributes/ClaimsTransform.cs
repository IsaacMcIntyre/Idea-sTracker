using System.Security.Claims;
using System.Threading.Tasks; 
using System.Linq;
using Microsoft.AspNetCore.Authentication;
using IdeasTracker.Constants;
using IdeasTracker.Database.Context;

namespace IdeasTracker.Attributes
{
    public class ClaimsTransformation : IClaimsTransformation
    {
        private readonly ApplicationDbContext _context;
        public ClaimsTransformation(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal claimPrincipal)
        {
            if (!claimPrincipal.Identity.IsAuthenticated)
                return await Task.FromResult(claimPrincipal);

            var identity = claimPrincipal.Identity as ClaimsIdentity;
            var emailClaim = FindClaim(claimPrincipal, "Email");
            if (emailClaim == null)
                return await Task.FromResult(claimPrincipal);

            var user = _context.Users.FirstOrDefault(x => x.Email.ToLower().Trim() == emailClaim.Value.Trim().ToLower());
            identity.AddClaim(new Claim(ClaimTypes.Role, user == null ? Roles.Andi : user.Role));

            return await Task.FromResult(claimPrincipal);
        }
        public static Claim FindClaim(ClaimsPrincipal claimPrincipal, string claimType)
        { 
            return claimPrincipal?.FindFirst(claimType);
        }
    }
}
