using System.Collections.Generic;
using System.Threading.Tasks;
using GuidesFusion360Server.Dtos.Users;
using GuidesFusion360Server.Models;

namespace GuidesFusion360Server.Services
{
    /// <summary>Service to process users requests.</summary>
    public interface IUsersService
    {
        /// <summary>Requests user self access.</summary>
        /// <returns>Returns user access.</returns>
        Task<ServiceResponse<string>> GetUserAccessData();

        /// <summary>Requests JWT token based on login info.</summary>
        /// <param name="email">Email of the user.</param>
        /// <param name="password">Raw password of the user.</param>
        /// <returns>Returns JWT token on success.</returns>
        Task<ServiceResponse<string>> GetUserToken(string email, string password);

        /// <summary>Requests to create new user in system.</summary>
        /// <param name="userData">User data.</param>
        /// <returns>Returns JWT token on success.</returns>
        Task<ServiceResponse<string>> CreateNewUser(UserRegisterDto userData);

        /// <summary>Requests password restoration.</summary>
        /// <param name="restoreCode">Code to restore password.</param>
        /// <param name="password">New password.</param>
        /// <returns>Returns JWT token.</returns>
        Task<ServiceResponse<string>> RestorePassword(string restoreCode, string password);

        /// <summary>Requests all users data.</summary>
        /// <returns>Returns all users data.</returns>
        Task<ServiceResponse<List<GetUserDto>>> GetUsers();

        /// <summary>Requests guides that user own.</summary>
        /// <param name="userId">Id of the user whose guides are requested.</param>
        /// <returns>Returns all user guides.</returns>
        Task<ServiceResponse<List<GetUserGuideDto>>> GetUserGuides(int userId);

        /// <summary>Requests password restore code.</summary>
        /// <param name="email">Email of user who need to restore the password.</param>
        /// <returns>Returns restore code.</returns>
        Task<ServiceResponse<string>> GetPasswordRestoreCode(string email);

        /// <summary>Requests to update user access.</summary>
        /// <param name="email">Email of the user who needs new access.</param>
        /// <param name="access">New access level.</param>
        /// <returns>Returns new access.</returns>
        Task<ServiceResponse<string>> UpdateUserAccess(string email, string access);

        /// <summary>Requests to delete user.</summary>
        /// <param name="email">Email of the user who will be deleted.</param>
        /// <returns>Returns number of guides that user had.</returns>
        Task<ServiceResponse<int>> DeleteUser(string email);
    }
}