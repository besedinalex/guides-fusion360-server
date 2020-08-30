using Microsoft.AspNetCore.Http;

namespace GuidesFusion360Server.Dtos
{
    public class UpdatePartGuideDto
    {
        public string Name { get; set; }

        public string Content { get; set; }

        public IFormFile File { get; set; }
    }
}