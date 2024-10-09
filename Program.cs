using FreelancePay.Contract;
using FreelancePay.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using FreelancePay.Contract.Repository;
using FreelancePay.Contract.Service;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace FreelancePay
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            

            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddHttpContextAccessor();
            //builder.Services.AddHttpClient();

            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
            builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
            builder.Services.AddScoped<IPaystackService, PaystackService>();


            builder.Services.AddHostedService<InvoiceBackgroundService>();
            builder.Services.AddHostedService<TransferBackgroundService>();


            // Add DB
            builder.Services.AddDbContext<AppDbContext>(
                options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
            );

            // Add authorization policies for freelancers and clients
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("Freelancer Policy", policy => policy.RequireRole(Role.Freelancer.ToString()));
                options.AddPolicy("Client Policy", policy => policy.RequireRole(Role.Client.ToString()));
            });

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options => {
                    options.LoginPath = "/Account/Login"; // Path to your login action
                    options.LogoutPath = "/Account/Logout"; // Path to your logout action
                });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication(); // Ensure authentication middleware is added
            app.UseAuthorization();

            app.MapRazorPages();

            // Map Login Route (Allow anonymous users)
            app.MapGet("/Login", () => Results.Redirect("/Account/Login")).AllowAnonymous();

            // Map Logout Route (Require authenticated users)
            app.MapPost("/Logout", async context =>
            {
                //context.Response.Cookies.Delete("claims");
                context.SignOutAsync();
                context.Response.Redirect("/");  // Use context.Response.Redirect instead of returning Results.Redirect
            }).RequireAuthorization();

            app.Run();
        }
    }
}
