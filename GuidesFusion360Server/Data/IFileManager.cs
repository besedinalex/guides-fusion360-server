using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace GuidesFusion360Server.Data
{
    /// <summary>Methods to work with guide files.</summary>
    public interface IFileManager
    {
        /// <summary>Checks if file exists.</summary>
        /// <param name="guideId">Id of the guide.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>Returns file existence status.</returns>
        bool FileExists(int guideId, string fileName);

        /// <summary>Saves guide file.</summary>
        /// <param name="guideId">Id of the guide.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="file">File from body form.</param>
        /// <returns>Returns 1 on success.</returns>
        Task<int> SaveFile(int guideId, string fileName, IFormFile file);
        
        /// <summary>Saves guide file.</summary>
        /// <param name="guideId">Id of the guide.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="file">File from array of bytes.</param>
        /// <returns>Returns 1 on success.</returns>
        Task<int> SaveFile(int guideId, string fileName, byte[] file);

        /// <summary>Returns file as byte array.</summary>
        /// <param name="guideId">Id of the guide.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>Returns file as byte array.</returns>
        Task<byte[]> GetFile(int guideId, string fileName);

        /// <summary>Deletes file/</summary>
        /// <param name="guideId">Id of the guide.</param>
        /// <param name="fileName">Name of the file.</param>
        void DeleteFile(int guideId, string fileName);
    }
}
