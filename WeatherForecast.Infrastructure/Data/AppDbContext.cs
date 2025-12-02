using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

using WeatherForecast.Domain.Models;
using WeatherForecast.Infrastructure.Configuration;
using WeatherForecast.Infrastructure.Models;

namespace WeatherForecast.Infrastructure.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }

    public DbSet<User> AppUsers => Set<User>();
    public DbSet<Favorite> Favorites => Set<Favorite>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfiguration(new FavoriteConfiguration());
        builder.ApplyConfiguration(new UserConfiguration());
    }
    
    }
