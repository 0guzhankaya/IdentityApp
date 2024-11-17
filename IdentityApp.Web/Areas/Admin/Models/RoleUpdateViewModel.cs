using System.ComponentModel.DataAnnotations;

namespace IdentityApp.Web.Areas.Admin.Models
{
	public class RoleUpdateViewModel
	{
		public string Id { get; set; } = null!;

		[Required(ErrorMessage = "Role alanı boş bırakılamaz!")]
		[Display(Name = "Role ismi")]
		public string Name { get; set; } = null!;
	}
}
