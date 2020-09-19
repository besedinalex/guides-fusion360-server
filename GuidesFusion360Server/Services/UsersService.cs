using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using GuidesFusion360Server.Data.Repositories;
using GuidesFusion360Server.Dtos.Users;
using GuidesFusion360Server.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace GuidesFusion360Server.Services
{
    /// <inheritdoc />
    public class UsersService : IUsersService
    {
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private readonly IUsersRepository _usersRepository;
        private readonly IGuidesRepository _guidesRepository;

        private int GetUserId
        {
            get
            {
                try
                {
                    return int.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
                }
                catch
                {
                    return -1;
                }
            }
        }

        private static readonly List<Tuple<string, string, DateTime>> RestorePasswordCodes;

        public UsersService(IMapper mapper, IHttpContextAccessor httpContextAccessor, IConfiguration configuration,
            IUsersRepository usersRepository, IGuidesRepository guidesRepository)
        {
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _usersRepository = usersRepository;
            _guidesRepository = guidesRepository;
        }

        static UsersService()
        {
            RestorePasswordCodes = new List<Tuple<string, string, DateTime>>();
        }

        /// <inheritdoc />
        public async Task<ServiceResponse<string>> GetUserAccessData()
        {
            var serviceResponse = new ServiceResponse<string>();

            var user = await _usersRepository.GetUser(GetUserId);
            if (user == null)
            {
                serviceResponse.StatusCode = 404;
                serviceResponse.Message = "User with given id was not found.";
                serviceResponse.MessageRu = "Пользователь с таким id не найден.";
                return serviceResponse;
            }

            serviceResponse.Data = user.Access;
            return serviceResponse;
        }

        /// <inheritdoc />
        public async Task<ServiceResponse<string>> GetUserToken(string email, string password)
        {
            var serviceResponse = new ServiceResponse<string>();

            var user = await _usersRepository.GetUser(email);
            if (user == null || !VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                serviceResponse.StatusCode = 404;
                serviceResponse.Message = "User with given email and password was not found.";
                serviceResponse.MessageRu = "Пользователь с указанными email и паролем не найден.";
                return serviceResponse;
            }

            serviceResponse.Data = CreateToken(user.Id, user.Email);
            return serviceResponse;
        }

        /// <inheritdoc />
        public async Task<ServiceResponse<string>> CreateNewUser(UserRegisterDto userData)
        {
            var serviceResponse = new ServiceResponse<string>();

            if (await _usersRepository.UserExists(userData.Email))
            {
                serviceResponse.StatusCode = 400;
                serviceResponse.Message = "User with this email already exists.";
                serviceResponse.MessageRu = "Пользователь с таким email уже существует.";
                return serviceResponse;
            }

            var user = new User
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

            serviceResponse.StatusCode = 201;
            serviceResponse.Data = CreateToken(userId, user.Email);
            serviceResponse.Message = "User is created successfully.";
            serviceResponse.MessageRu = "Пользователь успешно создан.";
            return serviceResponse;
        }

        /// <inheritdoc />
        public async Task<ServiceResponse<string>> RestorePassword(string restoreCode, string password)
        {
            var serviceResponse = new ServiceResponse<string>();

            var restoreData = RestorePasswordCodes.FirstOrDefault(x => x.Item1 == restoreCode);

            if (restoreData == null)
            {
                serviceResponse.StatusCode = 404;
                serviceResponse.Message = "Restore code is not found.";
                serviceResponse.MessageRu = "Код восстановления не найден.";
                return serviceResponse;
            }

            if (DateTime.Now > restoreData.Item3)
            {
                RestorePasswordCodes.Remove(restoreData);
                serviceResponse.StatusCode = 401;
                serviceResponse.Message = "Restore code is expired.";
                serviceResponse.MessageRu = "Код восстановления истек.";
                return serviceResponse;
            }

            var user = await _usersRepository.GetUser(restoreData.Item2);

            if (VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                serviceResponse.StatusCode = 400;
                serviceResponse.Message = "You cannot use the same password.";
                serviceResponse.MessageRu = "Вы не можете использовать тот же пароль.";
                return serviceResponse;
            }

            RestorePasswordCodes.Remove(restoreData);

            CreatePasswordHash(password, out var passwordHash, out var passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            await _usersRepository.UpdateUser(user);

            serviceResponse.Data = CreateToken(user.Id, user.Email);
            serviceResponse.Message = "Password changed successfully.";
            serviceResponse.MessageRu = "Пароль успешно обновлен.";
            return serviceResponse;
        }

        /// <inheritdoc />
        public async Task<ServiceResponse<List<GetUserDto>>> GetUsers()
        {
            var (isAdmin, accessResponse) = await RequesterIsAdmin<List<GetUserDto>>();
            if (!isAdmin)
            {
                return accessResponse;
            }

            var users = await _usersRepository.GetUsers();

            return new ServiceResponse<List<GetUserDto>>
            {
                Data = users.Select(x => _mapper.Map<GetUserDto>(x)).ToList()
            };
        }

        /// <inheritdoc />
        public async Task<ServiceResponse<List<GetUserGuideDto>>> GetUserGuides(int userId)
        {
            var (isAdmin, accessResponse) = await RequesterIsAdmin<List<GetUserGuideDto>>();
            if (!isAdmin)
            {
                return accessResponse;
            }

            var serviceResponse = new ServiceResponse<List<GetUserGuideDto>>();

            var user = await _usersRepository.GetUser(userId);
            if (user == null)
            {
                serviceResponse.StatusCode = 404;
                serviceResponse.Message = "User is not found.";
                serviceResponse.MessageRu = "Пользователь не найден.";
                return serviceResponse;
            }

            var allGuides = await _guidesRepository.GetAllGuides();
            var userGuides = allGuides.Where(x => x.OwnerId == userId).ToList();

            serviceResponse.Data = userGuides.Select(x => _mapper.Map<GetUserGuideDto>(x)).ToList();
            return serviceResponse;
        }

        /// <inheritdoc />
        public async Task<ServiceResponse<string>> GetPasswordRestoreCode(string email)
        {
            var (isAdmin, accessResponse) = await RequesterIsAdmin<string>();
            if (!isAdmin)
            {
                return accessResponse;
            }

            var serviceResponse = new ServiceResponse<string>();

            if (!await _usersRepository.UserExists(email))
            {
                serviceResponse.StatusCode = 404;
                serviceResponse.Message = "User is not found.";
                serviceResponse.MessageRu = "Пользователь не найден.";
                return serviceResponse;
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
            serviceResponse.MessageRu = "Сообщите этот код восстановления пользователю.";
            return serviceResponse;
        }

        /// <inheritdoc />
        public async Task<ServiceResponse<string>> UpdateUserAccess(string email, string access)
        {
            var (isAdmin, accessResponse) = await RequesterIsAdmin<string>();
            if (!isAdmin)
            {
                return accessResponse;
            }

            var serviceResponse = new ServiceResponse<string>();

            if (access != "unknown" && access != "editor")
            {
                serviceResponse.StatusCode = 400;
                serviceResponse.Message = "New access should be 'unknown' or 'editor'.";
                serviceResponse.MessageRu = "Новый уровень доступа должен быть 'unknown' или 'editor'.";
                return serviceResponse;
            }

            var user = await _usersRepository.GetUser(email);

            if (user == null)
            {
                serviceResponse.StatusCode = 404;
                serviceResponse.Message = "User is not found.";
                serviceResponse.MessageRu = "Пользователь не найден.";
                return serviceResponse;
            }

            if (user.Id == GetUserId)
            {
                serviceResponse.StatusCode = 400;
                serviceResponse.Message = "You cannot change access level for yourself.";
                serviceResponse.MessageRu = "Нельзя изменить уровень доступа самому себе.";
                return serviceResponse;
            }

            if (user.Access == "admin")
            {
                serviceResponse.StatusCode = 400;
                serviceResponse.Message = "You cannot change access level of another admin.";
                serviceResponse.MessageRu = "Нельзя изменить уровень доступа другому администатору.";
                return serviceResponse;
            }

            user.Access = access;
            await _usersRepository.UpdateUser(user);

            serviceResponse.Data = access;
            serviceResponse.Message = "New access level is set.";
            serviceResponse.MessageRu = "Новый уровень доступа установлен.";
            return serviceResponse;
        }

        /// <inheritdoc />
        public async Task<ServiceResponse<int>> DeleteUser(string email)
        {
            var (isAdmin, accessResponse) = await RequesterIsAdmin<int>();
            if (!isAdmin)
            {
                return accessResponse;
            }

            var serviceResponse = new ServiceResponse<int>();

            var userId = GetUserId;
            var user = await _usersRepository.GetUser(email);

            if (user == null)
            {
                serviceResponse.StatusCode = 404;
                serviceResponse.Message = "User is not found.";
                serviceResponse.MessageRu = "Пользователь не найден.";
                return serviceResponse;
            }

            if (user.Id == userId)
            {
                serviceResponse.StatusCode = 400;
                serviceResponse.Message = "You cannot delete yourself.";
                serviceResponse.MessageRu = "Вы не можете удалить самого себя.";
                return serviceResponse;
            }

            if (user.Access == "admin")
            {
                serviceResponse.StatusCode = 400;
                serviceResponse.Message = "You cannot delete another admin.";
                serviceResponse.MessageRu = "Нельзя удалить другого администратора.";
                return serviceResponse;
            }

            var userGuides = await _guidesRepository.GetAllGuides();

            foreach (var guide in userGuides)
                guide.OwnerId = userId;

            await _guidesRepository.UpdateGuides(userGuides);
            await _usersRepository.RemoveUser(user);

            serviceResponse.Data = userGuides.Count;
            serviceResponse.Message = "User is removed.";
            serviceResponse.MessageRu = "Пользователь удален.";
            return serviceResponse;
        }

        /// <summary>Checks if requests is admin.</summary>
        /// <returns>Returns service response and http code.</returns>
        private async Task<Tuple<bool, ServiceResponse<T>>> RequesterIsAdmin<T>()
        {
            var serviceResponse = new ServiceResponse<T>();

            var requesterIsAdmin = await _usersRepository.UserIsAdmin(GetUserId);
            if (!requesterIsAdmin)
            {
                serviceResponse.StatusCode = 401;
                serviceResponse.Message = "User should be an admin to make a such request.";
                serviceResponse.MessageRu = "Пользователь должен быть Редактором или Администратором.";
                return new Tuple<bool, ServiceResponse<T>>(false, serviceResponse);
            }

            return new Tuple<bool, ServiceResponse<T>>(true, serviceResponse);
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

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));

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