﻿using IdentityApp.Web.Areas.Admin.Models;
using IdentityApp.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdentityApp.Web.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = "admin")]
	public class HomeController : Controller
	{
		private readonly UserManager<AppUser> _userManager;

		public HomeController(UserManager<AppUser> userManager)
		{
			_userManager = userManager;
		}

		public IActionResult Index()
		{
			return View();
		}

		[HttpGet]
		public async Task<IActionResult> UserList()
		{
			var userList = await _userManager.Users.ToListAsync();
			var userViewModelList = userList.Select(x => new UserViewModel()
			{
				Id = x.Id,
				Name = x.UserName,
				Email = x.Email,
			}).ToList();

			return View(userViewModelList);
		}
	}
}
