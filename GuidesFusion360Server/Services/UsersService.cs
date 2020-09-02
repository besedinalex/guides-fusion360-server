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

        private static readonly List<Tuple<string, string, DateTime>> RestorePasswordCodes;

        public UsersService(IConfiguration configuration, IUsersRepository usersRepository)
        {
            _configuration = configuration;
            _usersRepository = usersRepository;
        }

        static UsersService()
        {
            RestorePasswordCodes = new List<Tuple<string, string, DateTime>>();
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
            serviceResponse.Message = "User is created successfully.";
            return serviceResponse;
        }

        /// <inheritdoc />
        public async Task<Tuple<ServiceResponseModel<string>, int>> GetPasswordRestoreCode(string email, int userId)
        {
            var serviceResponse = new ServiceResponseModel<string>();

            var requesterIsAdmin = await _usersRepository.UserIsAdmin(userId);

            if (!requesterIsAdmin)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "User should be admin to make such a request.";
                return new Tuple<ServiceResponseModel<string>, int>(serviceResponse, 401);
            }

            if (!await _usersRepository.UserExists(email))
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "User is not found.";
                return new Tuple<ServiceResponseModel<string>, int>(serviceResponse, 404);
            }

            // This isn't quite secure but this is a student project and those guides are backed up anyway.  
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var restoreCode = new string(Enumerable.Repeat(chars, 8).Select(s => s[random.Next(s.Length)]).ToArray());
            var expireDate = DateTime.Now.AddMinutes(30.0);

            var existingCode = RestorePasswordCodes.FirstOrDefault(x => x.Item2 == email);
            if (existingCode != null)
                RestorePasswordCodes.Remove(existingCode);

            RestorePasswordCodes.Add(new Tuple<string, string, DateTime>(restoreCode, email, expireDate));

            serviceResponse.Data = restoreCode;
            serviceResponse.Message = "Send this restore code to the user.";
            return new Tuple<ServiceResponseModel<string>, int>(serviceResponse, 200);
        }

        /// <inheritdoc />
        public async Task<Tuple<ServiceResponseModel<string>, int>> RestorePassword(string restoreCode, string password)
        {
            var serviceResponse = new ServiceResponseModel<string>();

            var restoreData = RestorePasswordCodes.FirstOrDefault(x => x.Item1 == restoreCode);

            if (restoreData == null)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "Restore code is not found.";
                return new Tuple<ServiceResponseModel<string>, int>(serviceResponse, 404);
            }

            if (DateTime.Now > restoreData.Item3)
            {
                RestorePasswordCodes.Remove(restoreData);
                serviceResponse.Success = false;
                serviceResponse.Message = "Restore code is expired.";
                return new Tuple<ServiceResponseModel<string>, int>(serviceResponse, 401);
            }

            var user = await _usersRepository.GetUser(restoreData.Item2);

            if (VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "You cannot use the same password.";
                return new Tuple<ServiceResponseModel<string>, int>(serviceResponse, 400);
            }

            RestorePasswordCodes.Remove(restoreData);

            CreatePasswordHash(password, out var passwordHash, out var passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            await _usersRepository.UpdateUser(user);

            serviceResponse.Data = CreateToken(user.Id, user.Email);
            serviceResponse.Message = "Password changed successfully.";
            return new Tuple<ServiceResponseModel<string>, int>(serviceResponse, 200);
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