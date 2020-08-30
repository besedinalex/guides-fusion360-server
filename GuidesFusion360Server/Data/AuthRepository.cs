using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using GuidesFusion360Server.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace GuidesFusion360Server.Data
{
    /// <inheritdoc />
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;

        public AuthRepository(DataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        /// <inheritdoc />
        public async Task<ServiceResponseModel<string>> Register(UserModel user, string password)
        {
            var serviceResponse = new ServiceResponseModel<string>();

            if (await UserExists(user.Email))
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "User with this email already exists.";
                return serviceResponse;
            }

            CreatePasswordHash(password, out var passwordHash, out var passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            serviceResponse.Data = CreateToken(user);
            serviceResponse.Message = "User is successfully created.";
            return serviceResponse;
        }

        /// <inheritdoc />
        public async Task<ServiceResponseModel<string>> Login(string email, string password)
        {
            var serviceResponse = new ServiceResponseModel<string>();
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email.ToLower().Equals(email.ToLower()));
            if (user == null || !VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "User with given email or password was not found.";
            }
            else
            {
                serviceResponse.Data = CreateToken(user);
            }

            return serviceResponse;
        }

        /// <inheritdoc />
        public async Task<bool> UserIsEditor(int userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);

            return user != null && (user.Access == "editor" || user.Access == "admin");
        }

        /// <inheritdoc />
        public async Task<bool> UserIsAdmin(int userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);

            return user != null && user.Access == "admin";
        }

        /// <summary>Checks if user exists.</summary>
        /// <param name="email">Email of the user.</param>
        /// <returns>Returns user existence status.</returns>
        private async Task<bool> UserExists(string email) =>
            await _context.Users.AnyAsync(x => x.Email.ToLower() == email.ToLower());

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
            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != passwordHash[i])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>Creates JWT token.</summary>
        /// <param name="user">User data.</param>
        /// <returns>Returns JWT token.</returns>
        private string CreateToken(UserModel user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Email)
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