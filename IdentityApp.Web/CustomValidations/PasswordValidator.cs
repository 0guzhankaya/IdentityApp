using IdentityApp.Web.Models;
using Microsoft.AspNetCore.Identity;

namespace IdentityApp.Web.CustomValidations
{
	public class PasswordValidator : IPasswordValidator<AppUser>
	{
		public Task<IdentityResult> ValidateAsync(UserManager<AppUser> manager, AppUser user, string? password)
		{
			var errors = new List<IdentityError>();

			// password içerisinde userName ile ilgili bir data bulunamaz.
			// ! : compiler'a nullable olmayacağı bilgisini verir. Runtime'da bir etkisi yoktur.
			if (password!.ToLower().Contains(user.UserName!.ToLower()))
			{
				errors.Add(new() { Code = "PasswordContainUserName", Description = "Şifre alanı kullanıcı adı içeremez." });
			}

			if (password!.ToLower().StartsWith("1234"))
			{
				errors.Add(new() { Code = "PasswordContain1234", Description = "Şifre alanı ardışık sayı içeremez." });
			}

			if (errors.Any())
			{
				// FromResult, içerisine yazılan tipi Task ile wrap'lar.
				return Task.FromResult(IdentityResult.Failed(errors.ToArray()));
			}

			return Task.FromResult(IdentityResult.Success);
		}
	}
}
