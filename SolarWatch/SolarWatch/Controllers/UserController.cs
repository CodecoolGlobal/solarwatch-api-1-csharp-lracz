using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace WeatherApi.Controllers;

 [Authorize(Roles = "Admin")] // Apply authorization as needed
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;

        public UsersController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult GetAllUsers()
        {
            var users = _userManager.Users;
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(IdentityUser user)
        {
            var result = await _userManager.CreateAsync(user);
            if (result.Succeeded)
                return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);

            return BadRequest(result.Errors);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, IdentityUser user)
        {
            if (id != user.Id)
                return BadRequest();

            var existingUser = await _userManager.FindByIdAsync(id);
            if (existingUser == null)
                return NotFound();

            existingUser.UserName = user.UserName;
            // Update other properties as needed

            var result = await _userManager.UpdateAsync(existingUser);
            if (result.Succeeded)
                return NoContent();

            return BadRequest(result.Errors);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
                return NoContent();

            return BadRequest(result.Errors);
        }
    }