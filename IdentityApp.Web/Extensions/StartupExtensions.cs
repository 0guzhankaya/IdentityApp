using IdentityApp.Web.CustomValidations;
using IdentityApp.Web.Localization;
using IdentityApp.Web.Models;
using Microsoft.AspNetCore.Identity;

namespace IdentityApp.Web.Extensions
{
	public static class StartupExtensions
	{
		public static void AddIdentityWithExtension(this IServiceCollection services)
		{
			// Reset Password Token Lifetime.
			services.Configure<DataProtectionTokenProviderOptions>(options =>
			{
				options.TokenLifespan = TimeSpan.FromMinutes(15);
			});

			// Identity 
			services.AddIdentity<AppUser, AppRole>(options =>
			{
				options.User.RequireUniqueEmail = true;
				options.User.AllowedUserNameCharacters = "abcdefghijklmnoprstuvxyz1234567890_";

				options.Password.RequiredLength = 6;
				options.Password.RequireNonAlphanumeric = false;
				options.Password.RequireLowercase = true;
				options.Password.RequireUppercase = true;
				options.Password.RequireDigit = false;

				options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
				options.Lockout.MaxFailedAccessAttempts = 3;

			}).AddPasswordValidator<PasswordValidator>()
			  .AddUserValidator<UserValidator>()
			  .AddErrorDescriber<LocalizationIdentityErrorDescriber>()
			  .AddDefaultTokenProviders() // comes from identity's token.
			  .AddEntityFrameworkStores<AppDbContext>();
		}
	}
}
