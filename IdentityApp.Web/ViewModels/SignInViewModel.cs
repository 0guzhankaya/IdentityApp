﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace IdentityApp.Web.ViewModels
{
	public class SignInViewModel
	{
        public SignInViewModel()
        {

        }

        public SignInViewModel(string email, string password)
		{
			Email = email;
			Password = password;
		}

		[Required(ErrorMessage = "Email alanı boş bırakılamaz!")]
		[EmailAddress(ErrorMessage = "Email formatı yanlıştır!")]
		[Display(Name = "Email:")]
		public string Email { get; set; }

		[DataType(DataType.Password)]
		[Required(ErrorMessage = "Şifre alanı boş bırakılamaz!")]
		[Display(Name = "Şifre:")]
		public string Password { get; set; }

		[Display(Name = "Beni Hatırla")]
		public bool RememberMe { get; set; }
	}
}
