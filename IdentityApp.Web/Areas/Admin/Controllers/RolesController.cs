﻿using Azure.Core;
using IdentityApp.Web.Areas.Admin.Models;
using IdentityApp.Web.Extensions;
using IdentityApp.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdentityApp.Web.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = "admin")]
	public class RolesController : Controller
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly RoleManager<AppRole> _roleManager;

		public RolesController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
		{
			_userManager = userManager;
			_roleManager = roleManager;
		}

		public async Task<IActionResult> Index()
		{
			var roles = await _roleManager.Roles.Select(x => new RoleViewModel()
			{
				Id = x.Id,
				Name = x.Name!,
			}).ToListAsync();

			return View(roles);
		}

		[HttpGet]
		public IActionResult RoleCreate()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> RoleCreate(RoleCreateViewModel request)
		{
			var result = await _roleManager.CreateAsync(new AppRole() { Name = request.Name });

			if (!result.Succeeded)
			{
				ModelState.AddModelErrorList(result.Errors);
				return View();
			}

			TempData["success"] = $"{request.Name} rolü başarıyla oluşturuldu.";

			return RedirectToAction(nameof(RolesController.Index));
		}

		[HttpGet]
		public async Task<IActionResult> RoleUpdate(string id)
		{
			var roleToUpdate = await _roleManager.FindByIdAsync(id);

			if (roleToUpdate == null)
			{
				throw new Exception("HATA! Rol bulunamadı!");
			}

			return View(new RoleUpdateViewModel()
			{
				Id = roleToUpdate.Id,
				Name = roleToUpdate.Name,
			});
		}

		[HttpPost]
		public async Task<IActionResult> RoleUpdate(RoleUpdateViewModel request)
		{
			var roleToUpdate = await _roleManager.FindByIdAsync(request.Id);

			if (roleToUpdate == null)
			{
				throw new Exception("HATA! Rol bulunamadı!");
			}

			roleToUpdate.Name = request.Name;
			await _roleManager.UpdateAsync(roleToUpdate);

			TempData["success"] = $"{roleToUpdate.Name} rolü başarıyla oluşturuldu.";

			return View();
		}

		[HttpGet]
		public async Task<IActionResult> RoleDelete(string id)
		{
			var roleToDelete = await _roleManager.FindByIdAsync(id);

			if (roleToDelete == null)
			{
				throw new Exception("HATA! Rol bulunamadı!");
			}

			var result = await _roleManager.DeleteAsync(roleToDelete);

			if (!result.Succeeded)
			{
				throw new Exception(result.Errors.Select(x => x.Description).First());
			}

			TempData["success"] = $"{roleToDelete.Name} rolü başarıyla silindi.";

			return RedirectToAction(nameof(RolesController.Index));
		}

		[HttpGet]
		public async Task<IActionResult> AssignRoleToUser(string id)
		{
			var currentUser = await _userManager.FindByIdAsync(id);
			ViewBag.userId = currentUser.Id;

			if (currentUser == null)
			{
				throw new Exception("HATA! Kullanıcı bulunamadı!");
			}

			var roles = await _roleManager.Roles.ToListAsync();
			var rolesViewModelList = new List<AssignRoleToUserViewModel>();

			var userRoles = await _userManager.GetRolesAsync(currentUser);

			foreach (var role in roles)
			{
				var assignRoleToUserViewModel = new AssignRoleToUserViewModel()
				{
					Id = role.Id,
					Name = role.Name!,
				};

				if (userRoles.Contains(role.Name!))
				{
					assignRoleToUserViewModel.Exits = true;
				}

				rolesViewModelList.Add(assignRoleToUserViewModel);
			}

			return View(rolesViewModelList);
		}

		[HttpPost]
		public async Task<IActionResult> AssignRoleToUser(string userId, List<AssignRoleToUserViewModel> requestList)
		{
			var userToAssignRoles = await _userManager.FindByIdAsync(userId);

			foreach (var role in requestList)
			{
				if (role.Exits)
				{
					await _userManager.AddToRoleAsync(userToAssignRoles, role.Name);
				}
				else
				{
					await _userManager.RemoveFromRoleAsync(userToAssignRoles, role.Name);
				}
			}

			return RedirectToAction(nameof(HomeController.UserList), "Home");
		}
	}
}
