using IdentityApp.Web.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace IdentityApp.Web.ClaimProvider
{
	public class UserClaimProvider : IClaimsTransformation
	{
		private readonly UserManager<AppUser> _userManager;

		public UserClaimProvider(UserManager<AppUser> userManager)
		{
			_userManager = userManager;
		}

		public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
		{
			// ClaimIdentity, claim'in cookie'den oluştuğunu belirtir.
			// Kullanıcının kimlik bilgisi claim'lerden oluşmaktadır.
			var identityUser = principal.Identity as ClaimsIdentity;

			var currentUser = await _userManager.FindByNameAsync(identityUser.Name);

			if (String.IsNullOrEmpty(currentUser!.City))
			{
				return principal;
			}

			if (principal.HasClaim(x => x.Type != "city"))
			{
				Claim cityClaim = new Claim("city", currentUser.City);
				identityUser.AddClaim(cityClaim);
			}

			return principal;
		}
	}
}
