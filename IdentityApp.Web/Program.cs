using IdentityApp.Web.Extensions;
using IdentityApp.Web.Models;
using IdentityApp.Web.OptionsModels;
using IdentityApp.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Database Connection String
builder.Services.AddDbContext<AppDbContext>(options =>
{
	options.UseSqlServer(builder.Configuration.GetConnectionString("SqlCon"));
});

// Extension Static Methods.
builder.Services.AddIdentityWithExtension();

builder.Services.ConfigureApplicationCookie(options =>
{
	var cookieBuilder = new CookieBuilder();
	cookieBuilder.Name = "IdentityAppCookie";
	options.LoginPath = new PathString("/Home/SignIn");
	options.LogoutPath = new PathString("/Member/Logout"); // Logout i�in 2.y�ntem
	options.Cookie = cookieBuilder;
	options.ExpireTimeSpan = TimeSpan.FromDays(60);
	options.SlidingExpiration = true;
});

// Email settings configuration
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

// DI
builder.Services.AddScoped<IEmailService, EmailService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Area Routing
app.MapControllerRoute(
	name: "areas",
	pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
