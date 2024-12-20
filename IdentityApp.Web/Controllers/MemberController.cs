﻿using IdentityApp.Web.Extensions;
using IdentityApp.Web.Models;
using IdentityApp.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using NuGet.Versioning;

namespace IdentityApp.Web.Controllers
{
	[Authorize]
	public class MemberController : Controller
	{
		private readonly SignInManager<AppUser> _signInManager;
		private readonly UserManager<AppUser> _userManager;
		private readonly IFileProvider _fileProvider;

		public MemberController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, IFileProvider fileProvider)
		{
			_signInManager = signInManager;
			_userManager = userManager;
			_fileProvider = fileProvider;
		}

		[HttpGet]
		public async Task<IActionResult> Index()
		{
			var currentUser = await _userManager.FindByNameAsync(User.Identity!.Name!)!;
			var userViewModel = new UserViewModel
			{
				Email = currentUser!.Email,
				UserName = currentUser!.UserName,
				PhoneNumber = currentUser.PhoneNumber,
				PictureUrl = currentUser.Picture,
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
				ModelState.AddModelErrorList(resultChangePassword.Errors);
				return View();
			}

			await _userManager.UpdateSecurityStampAsync(currentUser); // security stamp must change.
			await _signInManager.SignOutAsync();
			await _signInManager.PasswordSignInAsync(currentUser, request.PasswordNew, true, false);
			TempData["success"] = "Şifreniz başarıyla değiştirilmiştir.";

			return View();
		}

		[HttpGet]
		public async Task<IActionResult> UserEdit()
		{
			ViewBag.genderList = new SelectList(Enum.GetNames(typeof(Gender)));
			var currentUser = await _userManager.FindByNameAsync(User.Identity!.Name!);
			var userEditViewModel = new UserEditViewModel()
			{
				UserName = currentUser.UserName!,
				Email = currentUser.Email!,
				PhoneNumber = currentUser.PhoneNumber!,
				BirthDate = currentUser.BirthDate,
				City = currentUser.City,
				Gender = currentUser.Gender,
			};

			return View(userEditViewModel);
		}

		[HttpPost]
		public async Task<IActionResult> UserEdit(UserEditViewModel request)
		{
			if (!ModelState.IsValid)
			{
				return View();
			}

			var currentUser = await _userManager.FindByNameAsync(User.Identity!.Name!);
			currentUser.UserName = request.UserName;
			currentUser.Email = request.Email;
			currentUser.PhoneNumber = request.PhoneNumber;
			currentUser.BirthDate = request.BirthDate;
			currentUser.City = request.City;
			currentUser.Gender = request.Gender;

			if (request.Picture != null && request.Picture.Length > 0)
			{
				var wwwrootFolder = _fileProvider.GetDirectoryContents("wwwroot");
				var randomFileName = $"{Guid.NewGuid().ToString()}{Path.GetExtension(request.Picture.FileName)}"; //.png, .jpg

				string newPicturePath = Path.Combine(wwwrootFolder.First(x => x.Name == "pictures").PhysicalPath!, randomFileName);

				using var stream = new FileStream(newPicturePath, FileMode.Create);
				await request.Picture.CopyToAsync(stream);

				currentUser.Picture = randomFileName;
			}

			var updateToUserResult = await _userManager.UpdateAsync(currentUser);

			if (!updateToUserResult.Succeeded)
			{
				ModelState.AddModelErrorList(updateToUserResult.Errors);
				return View();
			}

			await _userManager.UpdateSecurityStampAsync(currentUser); // SecurityStamp must change.

			// çıkış - giriş gerçekleştirilecek.
			await _signInManager.SignOutAsync();
			await _signInManager.SignInAsync(currentUser, true);

			TempData["success"] = "Başarıyla güncellendi.";

			return View();
		}

		public async Task<IActionResult> Logout()
		{
			await _signInManager.SignOutAsync();
			return RedirectToAction("Index", "Home");
		}

		public IActionResult AccessDenied(string ReturnUrl)
		{
			string message = string.Empty;
			message = "Yetki alanı dışı:/";
			ViewBag.message = message;
			return View();
		}

		[HttpGet]
		public IActionResult Claims()
		{
			var userClaimList = User.Claims.Select(c => new ClaimViewModel() 
			{ 
				Issuer = c.Issuer, Type = c.Type, Value = c.Value
			}).ToList();

			return View(userClaimList);
		}

		[Authorize(Policy = "AnkaraPolicy")]
		[HttpGet]
		public IActionResult AnkaraPage()
		{

			return View();
		}
	}
}
