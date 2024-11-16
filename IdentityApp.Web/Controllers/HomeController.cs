using IdentityApp.Web.Models;
using IdentityApp.Web.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using IdentityApp.Web.Extensions;
using IdentityApp.Web.Services;

namespace IdentityApp.Web.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly UserManager<AppUser> _userManager; // Comes from Identity API.
		private readonly SignInManager<AppUser> _signInManager; // Comes from Identity API.
		private readonly IEmailService _emailService;

		public HomeController(ILogger<HomeController> logger, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IEmailService emailService)
		{
			_logger = logger;
			_userManager = userManager;
			_signInManager = signInManager;
			_emailService = emailService;
		}

		public IActionResult Index()
		{
			return View();
		}

		public IActionResult Privacy()
		{
			return View();
		}

		[HttpGet]
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
				TempData["SuccessMessage"] = "Kay�t Ba�ar�l�.";
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
				ModelState.AddModelError(string.Empty, "Email veya �ifre yanl��!");
				return View();
			}

			var signInResult = await _signInManager.PasswordSignInAsync(user, request.Password, request.RememberMe, true);

			if (signInResult.Succeeded)
			{
				return Redirect(returnUrl!);
			}

			if (signInResult.IsLockedOut)
			{
				ModelState.AddModelErrorList(new List<string>() { "10 dakika boyunca giri� yapamazs�n�z!" });
				return View();
			}

			ModelState.AddModelErrorList(new List<string>() { $"Email veya �ifre yanl��!",
				$"Ba�ar�s�z giri� say�s�: {await _userManager.GetAccessFailedCountAsync(user)}"});

			return View();
		}

		[HttpGet]
		public IActionResult ForgetPassword()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> ForgetPassword(ForgetPasswordViewModel request)
		{
			// �ifre s�f�rlama i�lemleminin zaman�n� k�s�tlamak i�in token g�nderilir.
			// https://localhost:port?userId=123334523&token=jsdklfkejfks25325fsd

			var user = await _userManager.FindByEmailAsync(request.Email);

			if (user == null)
			{
				// data'lar ModelState �zerinden ba�ka yere ta��nmaz.
				// Redirect edilirse data kayb� ya�an�r. Request'ler stateless'd�r. Http Protokol kural�d�r.
				// data TempData ile tek seferlik ta��nabilir.
				ModelState.AddModelError(String.Empty, "Kullan�c� bulunamam��t�r!");
				return View();
			}

			string passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
			var passwordResetLink = Url.Action("ResetPassword", "Home", new
			{
				userId = user.Id,
				Token = passwordResetToken
			}, HttpContext.Request.Scheme);

			// Email Service
			await _emailService.SendResetPasswordEmail(passwordResetLink, user.Email);

			// ayn� sayfada kald���nda refresh yap�l�rsa yeni bir mail gider.
			// TempData ile veri tutuldu ve Redirect edildi.
			TempData["success"] = "�ifre yenileme linki e-posta adresinize g�nderildi.";
			return RedirectToAction(nameof(ForgetPassword));
		}

		[HttpGet]
		public IActionResult ResetPassword(string userId, string token)
		{
			TempData["userId"] = userId;
			TempData["token"] = token;

			return View();
		}

		[HttpPost]
		public async Task<IActionResult> ResetPassword(ResetPasswordViewModel request)
		{
			var userId = TempData["userId"];
			var token = TempData["token"];

			if (userId == null || token == null)
			{
				throw new Exception("Beklenmedik bir hata ger�ekle�ti!");
			} 

			var user = await _userManager.FindByIdAsync(userId.ToString()!);
			if (user == null)
			{
				ModelState.AddModelError(String.Empty, "Kullan�c� bulunamad�!");
				return View();
			}

			IdentityResult result = await _userManager.ResetPasswordAsync(user, token.ToString()!, request.Password);
			if (result.Succeeded)
			{
				TempData["success"] = "�ifre ba�ar�yla yenilenmi�tir.";
			}

			ModelState.AddModelErrorList(result.Errors.Select(x => x.Description).ToList());
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
