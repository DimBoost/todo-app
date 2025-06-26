using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Data;
using ToDoApp.Models;
using ToDoApp.Services.Identity;
using ToDoApp.Services.RegisterEmailSender;
using ToDoApp.Services.Todos;
using ToDoApp.Services.Todos.Interfaces;

namespace ToDoApp
{
    public class Program
    {
      
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            ConfigureServices(builder.Services, builder.Configuration);

            var app = builder.Build();
            ConfigureMiddleware(app);
            await SeedDataAsync(app);

            await app.RunAsync();
        }


        private static void ConfigureServices(IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite(config.GetConnectionString("DefaultConnection")));

            services.AddTransient<IEmailSender, DummyEmailSender>();

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = "/AccessDenied";
            });

            services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, CustomClaimsPrincipalFactory>();
            services.AddScoped<ITodoService, TodoService>();
            services.AddScoped<ITodoDeleteService, TodoDeleteService>();
            services.AddScoped<ITodoCreateService, TodoCreateService>();

            services.AddRazorPages();
            services.AddAuthorization();

            services.AddLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
            });
        }

        private static void ConfigureMiddleware(WebApplication app)
        {
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapRazorPages();
        }
        //
        // Erstellt erforderliche Rollen und einen Admin-Benutzer beim ersten Start der Anwendung
        private static async Task SeedDataAsync(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            string[] roles = new[] { "Admin", "User" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            var adminEmail = "admin@todoapp.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "Admin",
                    LastName = "User"
                };
                var result = await userManager.CreateAsync(adminUser, "Admin1234!");
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }
}