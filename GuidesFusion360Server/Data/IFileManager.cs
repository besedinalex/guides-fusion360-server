using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace GuidesFusion360Server.Data
{
    public interface IFileManager
    {
        bool FileExists(int guideId, string fileName);

        Task<int> SaveFile(int guideId, string fileName, IFormFile file);

        Task<byte[]> GetFile(int guideId, string fileName);

        FileStream GetFileStream(int guideId, string fileName);

        int DeleteFile(int guideId, string fileName);
    }
}
