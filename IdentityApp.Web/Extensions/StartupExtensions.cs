using IdentityApp.Web.Models;

namespace IdentityApp.Web.Extensions
{
	public static class StartupExtensions
	{
		public static void AddIdentityWithExtension(this IServiceCollection services)
		{
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

			}).AddEntityFrameworkStores<AppDbContext>();
		}
	}
}
