using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Library.Dtos;
using Library.Entities;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Library.Controllers
{
    [ApiController]
    [Route("/")]
    [AllowAnonymous]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<Users> _userManager;
        private readonly IConfiguration _configuration;

        public AuthenticationController(UserManager<Users> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        // Allows users to login to the service
        // Route: /login
        [HttpPost]
        [Route("login")]
        public async Task<ActionResult> LoginAsync(LoginDto dto)
        {
            var user = await _userManager.FindByIdAsync(dto.Id);

            if(user is null)
            {
                return BadRequest("Unable to find a user with given id");
            }
            if(!await _userManager.CheckPasswordAsync(user, dto.Password))
            {
                return BadRequest("Password is incorrect");
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var authClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            foreach(var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var jwtSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));

            var token = new JwtSecurityToken(
                claims: authClaims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["JWT:Time"])),
                signingCredentials: new SigningCredentials(jwtSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiresAt = token.ValidTo
            });
        }
    }
}
