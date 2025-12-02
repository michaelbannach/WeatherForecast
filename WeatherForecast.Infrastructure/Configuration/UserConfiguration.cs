using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using WeatherForecast.Domain.Models;
using WeatherForecast.Infrastructure.Models;

namespace WeatherForecast.Infrastructure.Configuration;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        
        builder.HasKey(e => e.Id);

        builder.Property(u => u.Id)
            .ValueGeneratedOnAdd()
            .IsRequired();
        
        builder.Property(e => e.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasOne<ApplicationUser>()
            .WithOne()
            .HasForeignKey<User>(e => e.ApplicationUserId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}
