using IdentityApp.Web.Models;
using System.ComponentModel.DataAnnotations;

namespace IdentityApp.Web.ViewModels
{
	public class UserEditViewModel
	{
		[Required(ErrorMessage = "Kullanıcı Ad alanı boş bırakılamaz!")]
		[Display(Name = "Kullanıcı Adı")]
		public string UserName { get; set; } = null!;

		[Required(ErrorMessage = "Email alanı boş bırakılamaz!")]
		[EmailAddress(ErrorMessage = "Yanlış Email formatı!")]
		[Display(Name = "Email")]
		public string Email { get; set; } = null!;

		[Required(ErrorMessage = "Telefon alanı boş bırakılamaz!")]
		[Display(Name = "Telefon")]
		public string PhoneNumber { get; set; } = null!;

		[DataType(DataType.Date)]
		[Display(Name = "Doğum Tarihi")]
		public DateTime? BirthDate { get; set; }

		[Display(Name = "Şehir")]
		public string? City { get; set; }

		[Display(Name = "Kullanıcı Fotoğrafı")]
		public IFormFile? Picture { get; set; }

		[Display(Name = "Cinsiyet")]
		public Gender? Gender { get; set; }
	}
}
