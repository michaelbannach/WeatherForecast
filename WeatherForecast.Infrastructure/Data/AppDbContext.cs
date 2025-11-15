using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

using WeatherForecast.Domain.Models;

namespace WeatherForecast.Infrastructure.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Favorite> Favorites => Set<Favorite>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Favorite>(e =>
        {
            e.HasKey(x => x.Id);

            e.Property(x => x.City)
                .IsRequired()
                .HasMaxLength(100);

            e.Property(x => x.Country)
                .IsRequired()
                .HasMaxLength(2);

            e.Property(x => x.UserId)
                .IsRequired();

            e.HasOne(x => x.ApplicationUser)
                .WithMany(u => u.Favorites)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}