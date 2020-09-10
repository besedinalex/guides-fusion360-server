using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using GuidesFusion360Server.Dtos.Users;
using GuidesFusion360Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GuidesFusion360Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _usersService;

        public UsersController(IUsersService usersService)
        {
            _usersService = usersService;
        }

        [HttpGet("access-self")]
        public async Task<IActionResult> GetUserAccess()
        {
            var serviceResponse = await _usersService.GetUserAccessData();

            return StatusCode(serviceResponse.StatusCode, serviceResponse.ToControllerResponse());
        }

        [AllowAnonymous]
        [HttpGet("token")]
        public async Task<IActionResult> GetUserToken([Required] string email, [Required] string password)
        {
            var serviceResponse = await _usersService.GetUserToken(email, password);

            return StatusCode(serviceResponse.StatusCode, serviceResponse.ToControllerResponse());
        }

        [AllowAnonymous]
        [HttpPost("new")]
        public async Task<IActionResult> CreateNewUser(UserRegisterDto newUser)
        {
            var serviceResponse = await _usersService.CreateNewUser(newUser);

            return StatusCode(serviceResponse.StatusCode, serviceResponse.ToControllerResponse());
        }

        [AllowAnonymous]
        [HttpPut("restore-password")]
        public async Task<IActionResult> RestorePassword([Required] string restoreCode, [Required] string password)
        {
            var serviceResponse = await _usersService.RestorePassword(restoreCode, password);

            return StatusCode(serviceResponse.StatusCode, serviceResponse.ToControllerResponse());
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllUsers()
        {
            var serviceResponse = await _usersService.GetUsers();

            return StatusCode(serviceResponse.StatusCode, serviceResponse.ToControllerResponse());
        }

        [HttpGet("guides/{userId}")]
        public async Task<IActionResult> GetUserGuides([Required] int userId)
        {
            var serviceResponse = await _usersService.GetUserGuides(userId);

            return StatusCode(serviceResponse.StatusCode, serviceResponse.ToControllerResponse());
        }

        [HttpGet("password-restore-code")]
        public async Task<IActionResult> GetRestorePasswordCode([Required] string email)
        {
            var serviceResponse = await _usersService.GetPasswordRestoreCode(email);

            return StatusCode(serviceResponse.StatusCode, serviceResponse.ToControllerResponse());
        }

        [HttpPut("access")]
        public async Task<IActionResult> UpdateUserAccess(UpdateUserAccessDto userData)
        {
            var serviceResponse = await _usersService.UpdateUserAccess(userData.Email, userData.Access);

            return StatusCode(serviceResponse.StatusCode, serviceResponse.ToControllerResponse());
        }

        [HttpDelete("user")]
        public async Task<IActionResult> DeleteUser([Required] string email)
        {
            var serviceResponse = await _usersService.DeleteUser(email);

            return StatusCode(serviceResponse.StatusCode, serviceResponse.ToControllerResponse());
        }
    }
}