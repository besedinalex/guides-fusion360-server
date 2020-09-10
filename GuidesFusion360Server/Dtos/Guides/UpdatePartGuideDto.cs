using Microsoft.AspNetCore.Http;

namespace GuidesFusion360Server.Dtos.Guides
{
    public class UpdatePartGuideDto
    {
        public string Name { get; set; }

        public string Content { get; set; }

        public IFormFile File { get; set; }
    }
}