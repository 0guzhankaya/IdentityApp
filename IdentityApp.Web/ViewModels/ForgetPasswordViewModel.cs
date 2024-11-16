using System.ComponentModel.DataAnnotations;

namespace IdentityApp.Web.ViewModels
{
	public class ForgetPasswordViewModel
	{
		[Required(ErrorMessage = "Email alanı boş bırakılamaz!")]
		[EmailAddress(ErrorMessage = "Hatalı Email formatı!")]
		[Display(Name = "Email:")]
		public string Email { get; set; }
	}
}
