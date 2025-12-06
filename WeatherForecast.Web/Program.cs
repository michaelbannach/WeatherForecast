using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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


builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtSection["Key"];
var jwtIssuer = jwtSection["Issuer"];
var jwtAudience = jwtSection["Audience"];

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});

builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

    options.UseMySql(
        connectionString,
        new MySqlServerVersion(new Version(8, 0, 21)),
        mySqlOptions =>
        {
            
            mySqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(5),
                errorNumbersToAdd: null
            );
        });
});

// Identity
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

// Policies
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey!)),
            ClockSkew = TimeSpan.Zero
        };
    });


builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("SuperUserOnly", policy =>
        policy.RequireRole("SuperUser"));
});

// Services & Repositories
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IFavoriteService, FavoriteService>();
builder.Services.AddHttpClient<IWeatherService, WeatherService>(c =>
    c.Timeout = TimeSpan.FromSeconds(10)
);

builder.Services.AddScoped<IFavoriteRepository, FavoriteRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Cookies
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.SlidingExpiration = true;
});

// MVC / Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();

    var db = services.GetRequiredService<AppDbContext>();

    const int maxAttempts = 5;
    for (var attempt = 1; attempt <= maxAttempts; attempt++)
    {
        try
        {
            Console.WriteLine($"### Running EF Core migrations... (Attempt {attempt}/{maxAttempts})");
            await db.Database.MigrateAsync();
            Console.WriteLine("### Migrations finished.");
            break;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while applying migrations (attempt {Attempt})", attempt);

            if (attempt == maxAttempts)
            {
                Console.WriteLine("### Max migration attempts reached. Rethrowing...");
                throw;
            }

            Console.WriteLine("### Database not ready yet. Waiting 5 seconds before retry...");
            await Task.Delay(TimeSpan.FromSeconds(5));
        }
    }

    try
    {
        Console.WriteLine("### Seeding roles...");
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        string[] roles = { "SuperUser", "User" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
        Console.WriteLine("### Role seeding finished.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while seeding roles.");
        throw;
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    // optional: zusätzliche Development-Seeds
    await DevelopmentSeeder.SeedAsync(app.Services);
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("Frontend"); 
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
