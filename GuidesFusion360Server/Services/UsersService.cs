using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using GuidesFusion360Server.Data;
using GuidesFusion360Server.Dtos;
using GuidesFusion360Server.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace GuidesFusion360Server.Services
{
    /// <inheritdoc />
    public class UsersService : IUsersService
    {
        private readonly IConfiguration _configuration;
        private readonly IUsersRepository _usersRepository;

        public UsersService(IConfiguration configuration, IUsersRepository usersRepository)
        {
            _configuration = configuration;
            _usersRepository = usersRepository;
        }

        /// <inheritdoc />
        public async Task<ServiceResponseModel<string>> GetUserToken(string email, string password)
        {
            var serviceResponse = new ServiceResponseModel<string>();

            var user = await _usersRepository.GetUser(email);
            if (user == null || !VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "User with given email or password was not found.";
                return serviceResponse;
            }

            serviceResponse.Data = CreateToken(user.Id, user.Email);
            return serviceResponse;
        }

        /// <inheritdoc />
        public async Task<ServiceResponseModel<string>> CreateNewUser(UserRegisterDto userData)
        {
            var serviceResponse = new ServiceResponseModel<string>();

            if (await _usersRepository.UserExists(userData.Email))
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "User with this email already exists.";
                return serviceResponse;
            }

            var user = new UserModel
            {
                Email = userData.Email,
                FirstName = userData.FirstName,
                LastName = userData.LastName,
                Access = "unknown"
            };

            CreatePasswordHash(userData.Password, out var passwordHash, out var passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            var userId = await _usersRepository.AddUser(user);

            serviceResponse.Data = CreateToken(userId, user.Email);
            serviceResponse.Message = "User is created successfully .";
            return serviceResponse;
        }

        /// <summary>Creates password hash.</summary>
        /// <param name="password">Raw password.</param>
        /// <param name="passwordHash">Password hash to secure password.</param>
        /// <param name="passwordSalt">Password salt to verify password in future.</param>
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using var hmac = new System.Security.Cryptography.HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        /// <summary>Check if entered password is the same as the one stored and secured in db.</summary>
        /// <param name="password">Raw password.</param>
        /// <param name="passwordHash">Secured password.</param>
        /// <param name="passwordSalt">Password salt to compare raw password and hash.</param>
        /// <returns>Returns 'true' if password are the same.</returns>
        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return !computedHash.Where((x, i) => x != passwordHash[i]).Any();
        }

        /// <summary>Creates JWT token.</summary>
        /// <param name="userId">Id of the user.</param>
        /// <param name="email">Email of the user.</param>
        /// <returns>Returns JWT token.</returns>
        private string CreateToken(int userId, string email)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(_configuration.GetSection("AppSettings:Token").Value));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}