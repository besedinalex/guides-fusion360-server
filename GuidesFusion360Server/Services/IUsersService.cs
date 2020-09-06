using System;
using System.Threading.Tasks;
using GuidesFusion360Server.Dtos;
using GuidesFusion360Server.Models;

namespace GuidesFusion360Server.Services
{
    /// <summary>Service to process users requests.</summary>
    public interface IUsersService
    {
        /// <summary>Requests user self access.</summary>
        /// <param name="userId">Id of the user.</param>
        /// <returns>Returns user access.</returns>
        Task<ServiceResponseModel<string>> GetUserAccess(int userId);

        /// <summary>Requests JWT token based on login info.</summary>
        /// <param name="email">Email of the user.</param>
        /// <param name="password">Raw password of the user.</param>
        /// <returns>Returns JWT token on success.</returns>
        Task<ServiceResponseModel<string>> GetUserToken(string email, string password);

        /// <summary>Requests to create new user in system.</summary>
        /// <param name="userData">User data.</param>
        /// <returns>Returns JWT token on success.</returns>
        Task<ServiceResponseModel<string>> CreateNewUser(UserRegisterDto userData);

        /// <summary>Requests password restore code.</summary>
        /// <param name="email">Email of user who need to restore the password.</param>
        /// <param name="userId">Id of the user who requests the code.</param>
        /// <returns>Returns restore code and http code.</returns>
        Task<Tuple<ServiceResponseModel<string>, int>> GetPasswordRestoreCode(string email, int userId);

        /// <summary>Requests password restoration.</summary>
        /// <param name="restoreCode">Code to restore password.</param>
        /// <param name="password">New password.</param>
        /// <returns>Returns JWT token and http code.</returns>
        Task<Tuple<ServiceResponseModel<string>, int>> RestorePassword(string restoreCode, string password);
    }
}