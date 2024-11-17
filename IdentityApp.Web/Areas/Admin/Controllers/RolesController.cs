using Azure.Core;
using IdentityApp.Web.Areas.Admin.Models;
using IdentityApp.Web.Extensions;
using IdentityApp.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdentityApp.Web.Areas.Admin.Controllers
{
	[Area("Admin")]
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
	}
}
