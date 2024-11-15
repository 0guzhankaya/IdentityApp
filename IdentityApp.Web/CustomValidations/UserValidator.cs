using IdentityApp.Web.Models;
using Microsoft.AspNetCore.Identity;

namespace IdentityApp.Web.CustomValidations
{
	public class UserValidator : IUserValidator<AppUser>
	{
		public Task<IdentityResult> ValidateAsync(UserManager<AppUser> manager, AppUser user)
		{
			var errors = new List<IdentityError>();

			// UserName'in ilk harfinin sayısal karakter olamayacağını kontrol eder.
			// out kullanılmayacağı için; _ ile memory'de yer tutmaması bildirilir. Ignores value.
			var isDigit = int.TryParse(user.UserName[0]!.ToString(), out _);

			if (isDigit)
			{
				errors.Add(new() { Code = "UserNameContainFirstLetterDigit", Description = "Kullanıcı adı sayısal değer ile başlayamaz!" });
			}

			if (errors.Any())
			{
				return Task.FromResult(IdentityResult.Failed(errors.ToArray()));
			}

			return Task.FromResult(IdentityResult.Success);
		}
	}
}
