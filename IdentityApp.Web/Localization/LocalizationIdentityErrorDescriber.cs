using Microsoft.AspNetCore.Identity;

namespace IdentityApp.Web.Localization
{
	public class LocalizationIdentityErrorDescriber : IdentityErrorDescriber
	{
		public override IdentityError DuplicateUserName(string userName)
		{
			return new() { Code = "DuplicateUserName", Description = "Bu kullanıcı adı kullanımdadır!" };
		}

		public override IdentityError DuplicateEmail(string email)
		{
			return new() { Code = "DuplicateEmail", Description = "Bu e-posta kullanımdadır!" };
		}

		public override IdentityError PasswordTooShort(int length)
		{
			return new() { Code = "PasswordTooShort", Description = $"Şifre en az 6 karakterli olmalıdır!" };
		}

		public override IdentityError InvalidEmail(string? email)
		{
			return new() { Code = "InvalidEmail", Description = $"Geçersiz e-posta adresi!" };
		}
	}
}
