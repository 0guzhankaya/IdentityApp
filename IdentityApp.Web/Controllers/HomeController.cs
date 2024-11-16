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
				return Redirect(returnUrl!);
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

		[HttpGet]
		public IActionResult ForgetPassword()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> ForgetPassword(ForgetPasswordViewModel request)
		{
			// Þifre sýfýrlama iþlemleminin zamanýný kýsýtlamak için token gönderilir.
			// https://localhost:port?userId=123334523&token=jsdklfkejfks25325fsd

			var user = await _userManager.FindByEmailAsync(request.Email);

			if (user == null)
			{
				// data'lar ModelState üzerinden baþka yere taþýnmaz.
				// Redirect edilirse data kaybý yaþanýr. Request'ler stateless'dýr. Http Protokol kuralýdýr.
				// data TempData ile tek seferlik taþýnabilir.
				ModelState.AddModelError(String.Empty, "Kullanýcý bulunamamýþtýr!");
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

			// ayný sayfada kaldýðýnda refresh yapýlýrsa yeni bir mail gider.
			// TempData ile veri tutuldu ve Redirect edildi.
			TempData["success"] = "Þifre yenileme linki e-posta adresinize gönderildi.";
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
				throw new Exception("Beklenmedik bir hata gerçekleþti!");
			} 

			var user = await _userManager.FindByIdAsync(userId.ToString()!);
			if (user == null)
			{
				ModelState.AddModelError(String.Empty, "Kullanýcý bulunamadý!");
				return View();
			}

			IdentityResult result = await _userManager.ResetPasswordAsync(user, token.ToString()!, request.Password);
			if (result.Succeeded)
			{
				TempData["success"] = "Þifre baþarýyla yenilenmiþtir.";
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
