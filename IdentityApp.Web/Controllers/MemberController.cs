using IdentityApp.Web.Extensions;
using IdentityApp.Web.Models;
using IdentityApp.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace IdentityApp.Web.Controllers
{
	[Authorize]
	public class MemberController : Controller
	{
		private readonly SignInManager<AppUser> _signInManager;
		private readonly UserManager<AppUser> _userManager;

		public MemberController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager)
		{
			_signInManager = signInManager;
			_userManager = userManager;
		}

		[HttpGet]
		public async Task<IActionResult> Index()
		{
			var currentUser = await _userManager.FindByNameAsync(User.Identity!.Name!);
			var userViewModel = new UserViewModel
			{
				Email = currentUser!.Email,
				UserName = currentUser!.UserName,
				PhoneNumber = currentUser.PhoneNumber,
			};
			return View(userViewModel);
		}

		[HttpGet]
		public IActionResult PasswordChange()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> PasswordChange(PasswordChangeViewModel request)
		{
			if (!ModelState.IsValid)
			{
				return View();
			}

			var currentUser = await _userManager.FindByNameAsync(User.Identity!.Name!);
			var checkOldPassword = await _userManager.CheckPasswordAsync(currentUser, request.PasswordOld);

			if (!checkOldPassword)
			{
				ModelState.AddModelError(string.Empty, "Girdiğiniz eski şifreniz yanlış!");
				return View();
			}

			var resultChangePassword = await _userManager.ChangePasswordAsync(currentUser, request.PasswordOld, request.PasswordNew);

			if (!resultChangePassword.Succeeded)
			{
				ModelState.AddModelErrorList(resultChangePassword.Errors.Select(x => x.Description).ToList());
				return View();
			}

			await _userManager.UpdateSecurityStampAsync(currentUser); // security stamp must change.
			await _signInManager.SignOutAsync();
			await _signInManager.PasswordSignInAsync(currentUser, request.PasswordNew, true, false);
			TempData["success"] = "Şifreniz başarıyla değiştirilmiştir.";

			return View();
		}

		public async Task<IActionResult> Logout()
		{
			await _signInManager.SignOutAsync();
			return RedirectToAction("Index", "Home");
		}
	}
}
