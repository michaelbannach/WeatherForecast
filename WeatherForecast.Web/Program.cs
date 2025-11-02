using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WeatherForecast.Data;
using WeatherForecast.Interfaces;
using WeatherForecast.Models;
using WeatherForecast.Repositories;
using WeatherForecast.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Konfiguration einladen
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

// Datenbankanbindung MySQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 21))
    )
);

// Identity mit Rollenverwaltung und Passwortrichtlinien
builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequireDigit = false;
        options.Password.RequiredLength = 6;
        options.Password.RequireUppercase = true;
        options.SignIn.RequireConfirmedAccount = false;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// Eigene Services registrieren (Dependency Injection)
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IFavoriteService, FavoriteService>();
builder.Services.AddScoped<IFavoriteRepository, FavoriteRepository>();

// HttpClient für WeatherService mit Timeout
builder.Services.AddHttpClient<IWeatherService, WeatherService>(c => c.Timeout = TimeSpan.FromSeconds(10));

// Cookie-basierte Authentifizierung konfigurieren
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.SlidingExpiration = true;
});

builder.Services.AddControllers();

var app = builder.Build();

// Rollen sicherstellen und Superuser zuweisen
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    string[] roles = { "Superuser", "User" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }



// HTTP Request Pipeline konfigurieren
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
    }

    app.UseHttpsRedirection();

    app.UseCors(builder =>
        builder.WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
    );

    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}