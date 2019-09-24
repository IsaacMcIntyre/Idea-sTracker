using System;
using System.Security.Claims;
using System.Threading.Tasks;
using IdeasTracker.Data;
using System.Linq;
using Microsoft.AspNetCore.Authentication;
using IdeasTracker.Business.Enums;

namespace IdeasTracker.Attributes
{
    public class ClaimsTransformation : IClaimsTransformation
    {
        private readonly ApplicationDbContext _context;
        public ClaimsTransformation(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            var identity = principal.Identity as ClaimsIdentity;

            var user = _context.Users.FirstOrDefault(x => x.Name.ToLower().Trim() == identity.Name.ToLower().Trim());
            identity.AddClaim(new Claim(ClaimTypes.Role, user == null ? Roles.Andi : user.Role));

            return await Task.FromResult(principal);
        }
    }
}
