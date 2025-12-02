using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using WeatherForecast.Web.Dtos.Auth;
using WeatherForecast.Infrastructure.Models;

namespace WeatherForecast.Web.Controllers;


    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserById(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if(user == null) return NotFound();
            return Ok(user);
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var user = new ApplicationUser { UserName = dto.Email, Email = dto.Email };
            var result = await _userManager.CreateAsync(user, dto.Password);
            if(!result.Succeeded) return BadRequest(result.Errors);

            var role = string.IsNullOrEmpty(dto.Role) ? "User" : dto.Role;
            if(!await _roleManager.RoleExistsAsync(role)) 
                await _roleManager.CreateAsync(new IdentityRole(role));
            
            await _userManager.AddToRoleAsync(user, role);

            return Ok("User created");
        }
    }

  

