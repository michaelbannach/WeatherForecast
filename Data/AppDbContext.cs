using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using WeatherForecast.Models;

namespace WeatherForecast.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    
}