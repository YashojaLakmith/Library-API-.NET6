using Library.Dtos;
using Library.Entities;
using Library.Extensions;
using Library.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Library.Controllers
{
    [ApiController]
    [Route("users/")]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<Users> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUserServices _userServices;

        public UsersController(UserManager<Users> userManager, RoleManager<IdentityRole> roleManager, IUserServices userServices)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _userServices = userServices;
        }

        // Registers a new user
        // Route: /users/register
        [HttpPost]
        [Authorize(Roles = UserRoles.Sys_Admin)]
        [Route("register")]
        public async Task<ActionResult> RegisterUserAsync([FromBody] RegisterUserDto dto)
        {
            if(dto.Id.Equals(ReservedTerms.ReservedSystemId, StringComparison.OrdinalIgnoreCase) || dto.Name.Equals(ReservedTerms.ReservedSystemName, StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Reserved names has been used for user id and name");
            }

            var thisUser = await _userManager.GetUserAsync(HttpContext.User);
            if (!thisUser.IsActive)
            {
                return Forbid("Account is disabled");                           // Forbidden result
            }

            var existingUser = await _userManager.FindByIdAsync(dto.Id);
            if(existingUser is not null)
            {
                return BadRequest("A user with given id already exists");
            }

            if(((!dto.Role.Equals(UserRoles.Admin, StringComparison.OrdinalIgnoreCase))
                && (!dto.Role.Equals(UserRoles.User, StringComparison.OrdinalIgnoreCase)))
                || dto.Role.Equals(UserRoles.System, StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Specified role is not valid");
            }

            if(!HttpContext.User.IsInRole(UserRoles.System) && dto.Role.Equals(UserRoles.Admin, StringComparison.OrdinalIgnoreCase))
            {
                return Forbid("You must have System level permissions to create admin accounts");
            }

            var addressInfo = dto.Address;
            UserAddresses address = new()
            {
                Id = $"{addressInfo.District}-{addressInfo.City}-{addressInfo.Street1}-{dto.Id.ToUpper()}",
                District = addressInfo.District,
                City = addressInfo.City,
                Street1 = addressInfo.Street1,
                Street2 = addressInfo.Street2,
                Street3 = addressInfo.Street3
            };

            Users newUser = new()
            {
                Id = dto.Id.ToUpper(),
                UserName = dto.Name,
                SecurityStamp = Guid.NewGuid().ToString(),
                CreatedOn = DateTime.UtcNow,
                IsActive = true,
                Address = address,
            };

            var result = await _userManager.CreateAsync(newUser, dto.Password);
            if (!result.Succeeded)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            if(dto.Role.Equals(UserRoles.User, StringComparison.OrdinalIgnoreCase))
            {
                if (!await _roleManager.RoleExistsAsync(UserRoles.User))
                {
                    await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));
                }
                if(await _roleManager.RoleExistsAsync(UserRoles.User))
                {
                    await _userManager.AddToRoleAsync(newUser, UserRoles.User);         // change clogic
                }
                return NoContent();                                                     // change return type
            }

            if(dto.Role.Equals(UserRoles.Admin, StringComparison.OrdinalIgnoreCase))
            {
                if(!await _roleManager.RoleExistsAsync(UserRoles.Admin))
                {
                    await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
                }
                if(await _roleManager.RoleExistsAsync(UserRoles.Admin))
                {
                    await _userManager.AddToRoleAsync(newUser, UserRoles.Admin);        // change clogic
                }
            }
                                                                                        // add logic
            return CreatedAtAction(nameof(GetUserByIdAsync), new {id = dto.Id}, newUser.AsSingleUserDto());
        }

        // Gets a list of all registered users with limited information
        // Route: /users/
        [HttpGet]
        [Authorize(Roles = UserRoles.Sys_Admin)]
        public async Task<IEnumerable<UserListViewDto>> GetAllUsersAsync([FromQuery] string? bookId = null, bool? isActive = null)
        {
            if (!HttpContext.User.IsInRole(UserRoles.System))
            {
                return (await _userServices.GetAllUsersAsync(bookId, isActive))
                                            .Where(u => !u.Id.Equals(ReservedTerms.ReservedSystemId))
                                            .Select(u => u.AsUserListView())
                                            .ToList();
            }
            return (await _userServices.GetAllUsersAsync(bookId, isActive))
                                            .Select(u => u.AsUserListView())
                                            .ToList();
        }

        // Gets selected user information in a detailed manner
        // Route: /users/*userId*
        [HttpGet]
        [Authorize]
        [Route("{id}")]
        public async Task<ActionResult<SingleUserViewDto>> GetUserByIdAsync([FromRoute] string id)
        {
            if(!HttpContext.User.IsInRole(UserRoles.System) && id == ReservedTerms.ReservedSystemId)
            {
                return Forbid("You don't have permission to view system user");
            }

            var user = await _userServices.GetUserByIdAsync(id);
            
            if(user is null)
            {
                return NotFound("Couldn't find a user with given id");
            }

            var existingUserRoles = await _userManager.GetRolesAsync(user);
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);

            if(HttpContext.User.IsInRole(UserRoles.User) && !currentUser.Id.Equals(user.Id))
            {
                return Forbid("You do not have permission to view this profile");
            }

            if(HttpContext.User.IsInRole(UserRoles.Admin) && existingUserRoles.Any(r => r.Equals(UserRoles.Admin)) && !currentUser.Id.Equals(user.Id))
            {
                return Forbid("You do not have permission to view other admin accounts");
            }

            if(!HttpContext.User.IsInRole(UserRoles.System) && existingUserRoles.Any(r => r.Equals(UserRoles.System)))
            {
                return Forbid("You don't have permission to view System user's account");
            }

            return user.AsSingleUserDto();
        }

        // Modifies user information
        // Route: /users/modify/*userId*
        [HttpPatch]
        [Authorize(Roles = UserRoles.Sys_Admin)]
        [Route("modify/{id}")]
        public async Task<ActionResult> ModifyUserInfoAsync([FromRoute]string id, [FromBody]ModifyUserDto dto)
        {
            if(id.Equals(ReservedTerms.ReservedSystemId, StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("You cannot modify System user account");
            }

            if(dto.Name == null || dto.Name.Equals(ReservedTerms.ReservedSystemName, StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Reserved name cannot be used as user name");
            }

            if(dto.Address is null && string.IsNullOrWhiteSpace(dto.Name))
            {
                return BadRequest("Invalid request. Both fields cannot be null or empty");
            }

            var user = await _userServices.GetUserByIdAsync(id);

            if(user is null)
            {
                return NotFound("Couldn't find the user with given id");
            }

            if (!string.IsNullOrWhiteSpace(dto.Name))
            {
                user.UserName = dto.Name;
            }

            if(!string.IsNullOrWhiteSpace(dto.IsActive))
            {
                var result = bool.TryParse(dto.IsActive, out var res);
                if (!result)
                {
                    return BadRequest("Value specified in IsActive is invalid");
                }

                user.IsActive = bool.Parse(dto.IsActive);
            }

            if(dto.Address != null)
            {
                UserAddresses newAddress = new()
                {
                    Id = $"{dto.Address.District}-{dto.Address.City}-{dto.Address.Street1}-{user.Id.ToUpper()}",
                    District = dto.Address.District,
                    City = dto.Address.City,
                    Street1 = dto.Address.Street1,
                    Street2 = dto.Address.Street2,
                    Street3 = dto.Address.Street3
                };

                user.Address = newAddress;
            }

            await _userServices.ModifyUserInfoAsync(user);

            return NoContent();
        }

        // Changes user account password
        // Route: /users/change-password/*userId*
        [HttpPatch]
        [Authorize]
        [Route("change-password/{id}")]
        public async Task<ActionResult> ChangePasswordAsync([FromRoute]string id, [FromBody]ChangePasswordDto dto)
        {
            if(string.IsNullOrWhiteSpace(dto.CurrentPassword) || string.IsNullOrWhiteSpace(dto.NewPassword) || string.IsNullOrWhiteSpace(dto.ConfirmNewPassword))
            {
                return BadRequest("All fields are required");
            }

            if(!dto.NewPassword.Equals(dto.ConfirmNewPassword))
            {
                return BadRequest("Password confirmation doesn't match");
            }

            if (dto.CurrentPassword.Equals(dto.NewPassword))
            {
                return BadRequest("Please provide different password from the current password");
            }

            var existingUser = await _userServices.GetUserByIdAsync(id);
            if(existingUser is null)
            {
                return NotFound("Couldn't find a user with given id");
            }

            var currentUser = await _userManager.GetUserAsync(HttpContext.User);

            if(!currentUser.Id.Equals(existingUser.Id))
            {
                return Forbid("You don't have permissions to modify another user's password");
            }

            if(await _userManager.CheckPasswordAsync(existingUser, dto.CurrentPassword))
            {
                return BadRequest("Password is incorrect");
            }

            await _userManager.ChangePasswordAsync(existingUser, dto.CurrentPassword, dto.NewPassword);

            return NoContent();
        }

        // Deletes selected user account
        // Route: /users/*userId*
        [HttpDelete]
        [Authorize(Roles = UserRoles.Sys_Admin)]
        [Route("{id}")]
        public async Task<ActionResult> DeleteUserAsync([FromRoute] string id)
        {
            var existingUser = await _userServices.GetUserByIdAsync(id);
            if(existingUser is null)
            {
                return NotFound("Couldn't find the user with given id");
            }

            if (existingUser.Id.Equals(ReservedTerms.ReservedSystemId))
            {
                return BadRequest("System user account cannot be deleted");
            }

            var currentUser = await _userManager.GetUserAsync(HttpContext.User);

            if(HttpContext.User.IsInRole(UserRoles.Admin) && !existingUser.Id.Equals(currentUser.Id))
            {
                return Forbid("You don't have permissions to delete this account");
            }

            if(existingUser.RentedBooks is not null)
            {
                if (existingUser.RentedBooks.Any())
                {
                    return Forbid("User have not returned all the rented books");
                }

                await _userManager.DeleteAsync(existingUser);
                return NoContent();
            }

            await _userManager.DeleteAsync(existingUser);
            return NoContent();
        }
    }
}
