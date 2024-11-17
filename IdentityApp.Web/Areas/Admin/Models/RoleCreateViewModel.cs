using System.ComponentModel.DataAnnotations;

namespace IdentityApp.Web.Areas.Admin.Models
{
	public class RoleCreateViewModel
	{
		[Required(ErrorMessage = "Role alanı boş bırakılamaz!")]
		[Display(Name = "Role ismi")]
		public string Name { get; set; } = null!;
	}
}
