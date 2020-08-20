using System.Threading.Tasks;
using GuidesFusion360Server.Models;

namespace GuidesFusion360Server.Data
{
    public interface IAuthRepository
    {
        Task<ServiceResponse<string>> Register(User user, string password);
        
        Task<ServiceResponse<string>> Login(string email, string password);
        
        Task<bool> UserExists(string email);
    }
}
