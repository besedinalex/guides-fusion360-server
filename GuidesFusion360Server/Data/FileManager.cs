using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace GuidesFusion360Server.Data
{
    /// <inheritdoc />
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

        /// <inheritdoc />
        public bool FileExists(int guideId, string fileName)
        {
            var filePath = Path.Combine(_storage, guideId.ToString(), fileName);
            return File.Exists(filePath);
        }

        /// <inheritdoc />
        public async Task<int> SaveFile(int guideId, string fileName, IFormFile file)
        {
            var guideFolder = Path.Combine(_storage, guideId.ToString());
            Directory.CreateDirectory(guideFolder);

            var filePath = Path.Combine(guideFolder, fileName);
            await using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);
            return 1;
        }

        /// <inheritdoc />
        public async Task<int> SaveFile(int guideId, string fileName, byte[] file)
        {
            var guideFolder = Path.Combine(_storage, guideId.ToString());
            Directory.CreateDirectory(guideFolder);

            var filePath = Path.Combine(guideFolder, fileName);
            await File.WriteAllBytesAsync(filePath, file);
            return 1;
        }

        /// <inheritdoc />
        public async Task<byte[]> GetFile(int guideId, string fileName)
        {
            var filePath = Path.Combine(_storage, guideId.ToString(), fileName);
            var file = await File.ReadAllBytesAsync(filePath);
            return file;
        }

        /// <inheritdoc />
        public void DeleteFile(int guideId, string fileName)
        {
            var filePath = Path.Combine(_storage, guideId.ToString(), fileName);
            File.Delete(filePath);
        }
    }
}
