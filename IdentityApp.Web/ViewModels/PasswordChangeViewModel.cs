using System.ComponentModel.DataAnnotations;

namespace IdentityApp.Web.ViewModels
{
	public class PasswordChangeViewModel
	{
		[DataType(DataType.Password)]
		[Required(ErrorMessage = "Şifre alanı boş bırakılamaz!")]
		[Display(Name = "Eski Şifre")]
		[MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır!")]
		public string PasswordOld { get; set; } = null!;

		[DataType(DataType.Password)]
		[Required(ErrorMessage = "Yeni Şifre alanı boş bırakılamaz!")]
		[Display(Name = "Yeni Şifre")]
		[MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır!")]
		public string PasswordNew { get; set; } = null!;

		[DataType(DataType.Password)]
		[Required(ErrorMessage = "Yeni Şifre tekrar alanı boş bırakılamaz!")]
		[Compare(nameof(PasswordNew), ErrorMessage = "Girilen şifreler eşleşmiyor!")]
		[Display(Name = "Yeni Şifre Tekrar")]
		[MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır!")]
		public string PasswordNewConfirm { get; set; } = null!;
	}
}
