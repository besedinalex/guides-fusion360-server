using System.Threading.Tasks;
using GuidesFusion360Server.Data;
using GuidesFusion360Server.Dtos;
using GuidesFusion360Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GuidesFusion360Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IAuthRepository _authRepo;

        public UsersController(IAuthRepository authRepository)
        {
            _authRepo = authRepository;
        }

        [AllowAnonymous]
        [HttpGet("token")]
        public async Task<IActionResult> GetUserToken(UserLoginDto loginData)
        {
            var serviceResponse = await _authRepo.Login(loginData.Email, loginData.Password);

            if (!serviceResponse.Success)
            {
                return NotFound(serviceResponse);
            }

            return Ok(serviceResponse);
        }

        [AllowAnonymous]
        [HttpPost("new")]
        public async Task<IActionResult> CreateNewUser(UserRegisterDto newUser)
        {
            var user = new User()
            {
                Email = newUser.Email,
                FirstName = newUser.FirstName,
                LastName = newUser.LastName,
                Access = "unknown"
            };

            var serviceResponse = await _authRepo.Register(user, newUser.Password);

            if (!serviceResponse.Success)
            {
                return BadRequest(serviceResponse);
            }

            return Ok(serviceResponse);
        }
    }
}
