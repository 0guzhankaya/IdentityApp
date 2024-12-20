﻿using System.ComponentModel.DataAnnotations;

namespace IdentityApp.Web.ViewModels
{
	public class ResetPasswordViewModel
	{
		[DataType(DataType.Password)]
		[Required(ErrorMessage = "Şifre alanı boş bırakılamaz!")]
		[Display(Name = "Yeni Şifre")]
		public string Password { get; set; }

		[DataType(DataType.Password)]
		[Required(ErrorMessage = "Şifre tekrar alanı boş bırakılamaz!")]
		[Compare(nameof(Password), ErrorMessage = "Girilen şifreler eşleşmiyor!")]
		[Display(Name = "Yeni Şifre Tekrar")]
		public string PasswordConfirm { get; set; }
	}
}
