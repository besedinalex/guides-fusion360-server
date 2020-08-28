using System.Threading.Tasks;
using GuidesFusion360Server.Models;

namespace GuidesFusion360Server.Data
{
    /// <summary>Repository to work with user auth data.</summary>
    public interface IAuthRepository
    {
        /// <summary>Registers user in db.</summary>
        /// <param name="user">User data.</param>
        /// <param name="password">User password.</param>
        /// <returns>Returns user token.</returns>
        Task<ServiceResponse<string>> Register(User user, string password);
        
        /// <summary>Requests user token.</summary>
        /// <param name="email">User email.</param>
        /// <param name="password">User password.</param>
        /// <returns>Returns user token.</returns>
        Task<ServiceResponse<string>> Login(string email, string password);

        /// <summary>Checks if user is editor or admin.</summary>
        /// <param name="userId">Id of the user.</param>
        /// <returns>Returns 'true' if user is editor or admin.</returns>
        Task<bool> UserIsEditor(int userId);

        /// <summary>Checks if user is admin.</summary>
        /// <param name="userId">Id of the user.</param>
        /// <returns>Returns 'true' if user is admin.</returns>
        Task<bool> UserIsAdmin(int userId);
    }
}
