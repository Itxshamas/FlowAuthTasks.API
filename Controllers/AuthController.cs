using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using FlowAuthTasks.API.Models;
using FlowAuthTasks.API.DTOs.Auth;
using FlowAuthTasks.API.Helpers;

namespace FlowAuthTasks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JwtHelper _jwtHelper;

        public AuthController(UserManager<ApplicationUser> userManager, JwtHelper jwtHelper)
        {
            _userManager = userManager;
            _jwtHelper = jwtHelper;
        }

        // 🔹 Register
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FullName = dto.FullName
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            // Assign default role
            await _userManager.AddToRoleAsync(user, "User");

            return Ok("User registered successfully");
        }

        // 🔹 Login
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
                return Unauthorized("Invalid credentials");

            var roles = await _userManager.GetRolesAsync(user);

            var token = _jwtHelper.GenerateToken(user, roles);

            return Ok(new
            {
                token,
                email = user.Email,
                roles
            });
        }
    }
} 