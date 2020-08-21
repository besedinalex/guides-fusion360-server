using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GuidesFusion360Server.Data
{
    public interface IFileManager
    {
        bool FileExists(int guideId, string fileName);

        Task<int> SaveFile(int guideId, string fileName, IFormFile file);

        Task<FileContentResult> GetFile(int guideId, string fileName, string contentType);
    }
}
