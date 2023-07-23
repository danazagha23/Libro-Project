using Libro.Domain.Entities;
using Libro.Infrastructure.Data.DbContexts;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Tests.Common
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Replace your DbContext registration with an in-memory database
                services.AddDbContext<LibroDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDb");
                });

                // Configure authentication
                services.AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                }).AddCookie(options =>
                {
                    options.Cookie.Name = "TestCookie";
                    options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax;
                    options.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.None; // Adjust as needed for your testing environment
                });

                // Create a new scope to obtain a reference to the service provider
                using (var scope = services.BuildServiceProvider().CreateScope())
                {
                    var serviceProvider = scope.ServiceProvider;
                    var dbContext = serviceProvider.GetRequiredService<LibroDbContext>();
                    var logger = serviceProvider.GetRequiredService<ILogger<CustomWebApplicationFactory<TStartup>>>();

                    try
                    {
                        // Seed the in-memory database with test data
                        // You can add your own seeding logic here
                        dbContext.Database.EnsureCreated();

                        // Example: Add a test user
                        dbContext.Users.Add(new User { UserId = 1, Username = "testuser" });
                        dbContext.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "An error occurred while seeding the database with test data.");
                    }
                }
            });
        }

        public HttpClient CreateAuthenticatedClient(string username, string role)
        {
            var client = CreateClient();
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "1"),
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, role)
        };
            var claimsIdentity = new ClaimsIdentity(claims, "TestAuthentication");
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("TestAuthentication");
            client.DefaultRequestHeaders.Add("username", username);
            client.DefaultRequestHeaders.Add("role", role);
            return client;
        }
    }
}
