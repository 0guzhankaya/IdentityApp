﻿using IdentityApp.Web.OptionsModels;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace IdentityApp.Web.Services
{
	public class EmailService : IEmailService
	{
		private readonly EmailSettings _emailSettings;

		public EmailService(IOptions<EmailSettings> emailSettings)
		{
			_emailSettings = emailSettings.Value;
		}

		public async Task SendResetPasswordEmail(string resetPasswordEmailLink, string ToEmail)
		{
			var smtpClient = new SmtpClient();

			// smtp configuration
			smtpClient.Host = _emailSettings.Host;
			smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
			smtpClient.UseDefaultCredentials = false;
			smtpClient.Port = 587;
			smtpClient.Credentials = new NetworkCredential(_emailSettings.Email, _emailSettings.Password);
			smtpClient.EnableSsl = true;

			// write mail
			var mailMessage = new MailMessage();
			mailMessage.From = new MailAddress(_emailSettings.Email);
			mailMessage.To.Add(ToEmail);
			mailMessage.Subject = "IdentityApp Şifre Sıfırlama Linki";
			mailMessage.Body = @$"
								<h4>Şifrenizi yenilemek için aşağıdaki linke tıklayınız.</h4>
								<p>
									<a href='{resetPasswordEmailLink}'>Şifremi yenile</a>
								</p>";
			mailMessage.IsBodyHtml = true;
			await smtpClient.SendMailAsync(mailMessage);
		}
	}
}