using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

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

        public bool FileExists(int guideId, string fileName)
        {
            var filePath = Path.Combine(_storage, guideId.ToString(), fileName);
            return File.Exists(filePath);
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

        public async Task<byte[]> GetFile(int guideId, string fileName)
        {
            var filePath = Path.Combine(_storage, guideId.ToString(), fileName);
            var file = await File.ReadAllBytesAsync(filePath);
            return file;
        }

        public FileStream GetFileStream(int guideId, string fileName)
        {
            var filePath = Path.Combine(_storage, guideId.ToString(), fileName);
            var fs = File.OpenRead(filePath);
            return fs;
        }

        public int DeleteFile(int guideId, string fileName)
        {
            var filePath = Path.Combine(_storage, guideId.ToString(), fileName);
            File.Delete(filePath);
            return 0;
        }
    }
}
