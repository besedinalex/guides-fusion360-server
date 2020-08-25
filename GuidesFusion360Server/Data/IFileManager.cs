using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace GuidesFusion360Server.Data
{
    public interface IFileManager
    {
        bool FileExists(int guideId, string fileName);

        Task<int> SaveFile(int guideId, string fileName, IFormFile file);
        
        Task<int> SaveFile(int guideId, string fileName, byte[] file);

        Task<byte[]> GetFile(int guideId, string fileName);

        void DeleteFile(int guideId, string fileName);
    }
}
