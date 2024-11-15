using IdentityApp.Web.Models;
using IdentityApp.Web.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using IdentityApp.Web.Extensions;

namespace IdentityApp.Web.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly UserManager<AppUser> _userManager; // Comes from Identity API.
		private readonly SignInManager<AppUser> _signInManager; // Comes from Identity API.

		public HomeController(ILogger<HomeController> logger, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
		{
			_logger = logger;
			_userManager = userManager;
			_signInManager = signInManager;
		}

		public IActionResult Index()
		{
			return View();
		}

		public IActionResult Privacy()
		{
			return View();
		}

		public IActionResult SignUp()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> SignUp(SignUpViewModel request)
		{
			if (!ModelState.IsValid)
			{
				return View();
			}

			var identityResult = await _userManager.CreateAsync(new()
			{
				UserName = request.UserName,
				PhoneNumber = request.PhoneNumber,
				Email = request.Email
			}, request.PasswordConfirm);

			if (identityResult.Succeeded)
			{
				TempData["SuccessMessage"] = "Kayýt Baþarýlý.";
				return RedirectToAction(nameof(HomeController.SignUp));
			}

			// returns error list.
			ModelState.AddModelErrorList(identityResult.Errors.Select(x => x.Description).ToList());

			return View();
		}

		[HttpGet]
		public IActionResult SignIn()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> SignIn(SignInViewModel request, string? returnUrl = null)
		{
			returnUrl = returnUrl ?? Url.Action("Index", "Home");
			var user = await _userManager.FindByEmailAsync(request.Email);

			if (user == null)
			{
				ModelState.AddModelError(string.Empty, "Email veya þifre yanlýþ!");
				return View();
			}

			var signInResult = await _signInManager.PasswordSignInAsync(user, request.Password, request.RememberMe, true);

			if (signInResult.Succeeded)
			{
				return Redirect(returnUrl);
			}

			if (signInResult.IsLockedOut)
			{
				ModelState.AddModelErrorList(new List<string>() { "10 dakika boyunca giriþ yapamazsýnýz!" });
				return View();
			}

			ModelState.AddModelErrorList(new List<string>() { $"Email veya þifre yanlýþ!", 
				$"Baþarýsýz giriþ sayýsý: {await _userManager.GetAccessFailedCountAsync(user)}"});

			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
