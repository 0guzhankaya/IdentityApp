﻿using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace IdentityApp.Web.ViewModels
{
    public class SignUpViewModel
    {
        public SignUpViewModel()
        {
        }

        public SignUpViewModel(string? userName, string? email, string? phoneNumber, string? password)
        {
            UserName = userName;
            Email = email;
            PhoneNumber = phoneNumber;
            Password = password;
        }

        [Required(ErrorMessage = "Kullanıcı Ad alanı boş bırakılamaz!")]
        [Display(Name = "Kullanıcı Adı")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email alanı boş bırakılamaz!")]
        [EmailAddress(ErrorMessage = "Yanlış Email formatı!")]
		[Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Telefon alanı boş bırakılamaz!")]
        [Display(Name = "Telefon")]
        public string PhoneNumber { get; set; }

		[DataType(DataType.Password)]
		[Required(ErrorMessage = "Şifre alanı boş bırakılamaz!")]
        [Display(Name = "Şifre")]
		[MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır!")]
		public string Password { get; set; }

		[DataType(DataType.Password)]
		[Required(ErrorMessage = "Şifre tekrar alanı boş bırakılamaz!")]
        [Compare(nameof(Password), ErrorMessage = "Girilen şifreler eşleşmiyor!")]
        [Display (Name = "Şifre Tekrar")]
		[MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır!")]
		public string PasswordConfirm { get; set; }
    }
}
