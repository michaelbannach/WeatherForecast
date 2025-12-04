using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using WeatherForecast.Application.Services;
using WeatherForecast.Application.Interfaces;
using WeatherForecast.Infrastructure.Models;
using WeatherForecast.Infrastructure.Data;
using WeatherForecast.Infrastructure.Repositories;
using WeatherForecast.Infrastructure.Services;
using WeatherForecast.Infrastructure.Seeding;

var builder = WebApplication.CreateBuilder(args);

Console.WriteLine("### ENVIRONMENT = " + builder.Environment.EnvironmentName);
Console.WriteLine("### OpenWeatherMap:BaseUrl = '" + (builder.Configuration["OpenWeatherMap:BaseUrl"] ?? "<null>") + "'");
Console.WriteLine("### OpenWeatherMap:ApiKey = '" + (builder.Configuration["OpenWeatherMap:ApiKey"] ?? "<null>") + "'");

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
        policy
            .WithOrigins("http://localhost:5173") 
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});


builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 21))
    )
);


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

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("SuperUserOnly", policy =>
        policy.RequireRole("SuperUser"));
});

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IFavoriteService, FavoriteService>();
builder.Services.AddHttpClient<IWeatherService, WeatherService>(c => 
    c.Timeout = TimeSpan.FromSeconds(10)
);

builder.Services.AddScoped<IFavoriteRepository, FavoriteRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();


builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.SlidingExpiration = true;
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

await using var roleScope = app.Services.CreateAsyncScope();
var roleManager = roleScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
string[] roles = { "SuperUser", "User" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    using var scope = app.Services.CreateScope();
    await DevelopmentSeeder.SeedAsync(app.Services);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("Frontend"); //Zwingend vor Authentication und Authorization
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
