using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GuidesFusion360Server.Data
{
    public class FileManager : IFileManager
    {
        private readonly string _storage;

        public FileManager()
        {
            var platform = Environment.OSVersion.Platform;
            var homePath = (platform == PlatformID.Unix || platform == PlatformID.MacOSX)
                ? Environment.GetEnvironmentVariable("HOME")
                : Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");
            _storage = Path.Combine(homePath!, "Fusion360GuideStorage");
            Directory.CreateDirectory(_storage);
        }

        public async Task<int> SaveFile(int guideId, string fileName, IFormFile file)
        {
            var guideFolder = Path.Combine(_storage, guideId.ToString());
            Directory.CreateDirectory(guideFolder);

            var filePath = Path.Combine(guideFolder, fileName);
            await using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);
            return 1;
        }

        public async Task<FileContentResult> GetFile(int guideId, string fileName, string contentType)
        {
            var imagePath = Path.Combine(_storage, guideId.ToString(), fileName);
            var image = await File.ReadAllBytesAsync(imagePath);
            return new FileContentResult(image, contentType);
        }
    }
}
