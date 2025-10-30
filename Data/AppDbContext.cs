using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using WeatherForecast.Models;

namespace WeatherForecast.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<UserInput> UserInputs => Set<UserInput>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<UserInput>(e =>
        {
            e.HasKey(x => x.Id);

            e.Property(x => x.City)
                .IsRequired()
                .HasMaxLength(100);

            e.Property(x => x.Country)
                .IsRequired()
                .HasMaxLength(100);

            e.Property(x => x.UserId)
                .IsRequired();

            e.HasOne(x => x.ApplicationUser)
                .WithMany(u => u.UserInputs)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}