using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WeatherForecast.Infrastructure.Data; // Dein DbContext
using WeatherForecast.Infrastructure.Models;
using WeatherForecast.Domain.Models;
using WeatherForecast.Application.Interfaces;

namespace WeatherForecast.Infrastructure.Seeding;

public static class DevelopmentSeeder
{
    private static bool _initialized;
    private static readonly object _lock = new();

    public static async Task SeedAsync(IServiceProvider services)
    {
        if (_initialized) return;
        lock (_lock)
        {
            if (_initialized) return;
            _initialized = true;
        }

        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>(); 
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var userService = scope.ServiceProvider.GetRequiredService<IUserService>();

        // 1) SUPERUSER for Favorites (SuperUser Policy)
        const string userName = "superuser";
        const string email = "superuser@test.local";
        const string password = "Super123!";

        var superUser = await userManager.FindByNameAsync(userName) 
            ?? await userManager.FindByEmailAsync(email);

        if (superUser == null)
        {
            superUser = new ApplicationUser
            {
                UserName = userName,
                Email = email,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(superUser, password);
            if (!result.Succeeded)
                throw new Exception($"SuperUser creation failed: {string.Join(", ", result.Errors.Select(e => e.Description))}");

            // SuperUser Role
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            if (!await roleManager.RoleExistsAsync("SuperUser"))
                await roleManager.CreateAsync(new IdentityRole("SuperUser"));
            
            await userManager.AddToRoleAsync(superUser, "SuperUser");
        }

        // 2) Domain User for Favorites
        var domainUser = await db.Users.FirstOrDefaultAsync(u => u.ApplicationUserId == superUser.Id);
        if (domainUser == null)
        {
            domainUser = new User
            {
                ApplicationUserId = superUser.Id,
                FirstName = "Super",
                LastName = "User"
            };
            await userService.CreateUserAsync(domainUser);
        }

        // 3) TEST FAVORITE for Swagger
        if (!await db.Favorites.AnyAsync())
        {
            await db.Favorites.AddAsync(new Favorite
            {
                UserId = domainUser.Id,
                City = "München",
                Country = "DE"
            });
            await db.SaveChangesAsync();
        }
    }
}
