using System.Threading.Tasks;
using GuidesFusion360Server.Dtos;
using GuidesFusion360Server.Models;

namespace GuidesFusion360Server.Services
{
    /// <summary>Service to process users requests.</summary>
    public interface IUsersService
    {
        /// <summary>Requests JWT token based on login info.</summary>
        /// <param name="email">Email of the user.</param>
        /// <param name="password">Raw password of the user.</param>
        /// <returns>Returns JWT token on success.</returns>
        Task<ServiceResponseModel<string>> GetUserToken(string email, string password);

        /// <summary>Requests to create new user in system.</summary>
        /// <param name="userData">User data.</param>
        /// <returns>Returns JWT token on success.</returns>
        Task<ServiceResponseModel<string>> CreateNewUser(UserRegisterDto userData);
    }
}