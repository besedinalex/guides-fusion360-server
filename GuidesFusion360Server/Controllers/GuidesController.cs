using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using GuidesFusion360Server.Dtos.Guides;
using GuidesFusion360Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GuidesFusion360Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class GuidesController : ControllerBase
    {
        private readonly IGuidesService _guidesService;

        public GuidesController(IGuidesService guidesService)
        {
            _guidesService = guidesService;
        }

        [AllowAnonymous]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllPublicGuides()
        {
            var serviceResponse = await _guidesService.GetAllPublicGuides();

            return StatusCode(serviceResponse.StatusCode, serviceResponse.ToControllerResponse());
        }

        [HttpGet("all-hidden")]
        public async Task<IActionResult> GetAllHiddenGuides()
        {
            var serviceResponse = await _guidesService.GetAllHiddenGuides();

            return StatusCode(serviceResponse.StatusCode, serviceResponse.ToControllerResponse());
        }

        [AllowAnonymous]
        [HttpGet("parts-public/{guideId}")]
        public async Task<IActionResult> GetPublicPartGuides([Required] int guideId)
        {
            var serviceResponse = await _guidesService.GetPartGuides(guideId);

            return StatusCode(serviceResponse.StatusCode, serviceResponse.ToControllerResponse());
        }

        [HttpGet("parts/{guideId}")]
        public async Task<IActionResult> GetPrivatePartGuides([Required] int guideId)
        {
            var serviceResponse = await _guidesService.GetPartGuides(guideId);

            return StatusCode(serviceResponse.StatusCode, serviceResponse.ToControllerResponse());
        }

        [AllowAnonymous]
        [HttpGet("file-public/{guideId}")]
        public async Task<IActionResult> GetPublicGuideFile([Required] int guideId, [Required] string filename)
        {
            var serviceResponse = await _guidesService.GetGuideFile(guideId, filename);

            if (serviceResponse.StatusCode == 200)
            {
                return serviceResponse.Data;
            }

            return StatusCode(serviceResponse.StatusCode, serviceResponse.ToControllerResponse());
        }

        [HttpGet("file/{guideId}")]
        public async Task<IActionResult> GetPrivateGuideFile([Required] int guideId, [Required] string filename)
        {
            var serviceResponse = await _guidesService.GetGuideFile(guideId, filename);

            if (serviceResponse.StatusCode == 200)
            {
                return serviceResponse.Data;
            }

            return StatusCode(serviceResponse.StatusCode, serviceResponse.ToControllerResponse());
        }

        [HttpGet("owner/{guideId}")]
        public async Task<IActionResult> GetGuideOwner([Required] int guideId)
        {
            var serviceResponse = await _guidesService.GetGuideOwner(guideId);

            return StatusCode(serviceResponse.StatusCode, serviceResponse.ToControllerResponse());
        }

        [HttpPost("guide")]
        public async Task<IActionResult> CreateNewGuide([FromForm] AddGuideDto guide)
        {
            var serviceResponse = await _guidesService.CreateGuide(guide);

            return StatusCode(serviceResponse.StatusCode, serviceResponse.ToControllerResponse());
        }

        [DisableRequestSizeLimit]
        [HttpPost("part-guide")]
        public async Task<IActionResult> CreateNewPartGuide([FromForm] AddPartGuideDto guide)
        {
            var serviceResponse = await _guidesService.CreatePartGuide(guide);

            return StatusCode(serviceResponse.StatusCode, serviceResponse.ToControllerResponse());
        }

        [DisableRequestSizeLimit]
        [HttpPost("model")]
        public async Task<IActionResult> UploadModel([FromForm] AddGuideModelDto model)
        {
            var serviceResponse = await _guidesService.UploadModel(model);

            return StatusCode(serviceResponse.StatusCode, serviceResponse.ToControllerResponse());
        }

        [HttpPut("hidden/{guideId}")]
        public async Task<IActionResult> ChangeGuideVisibility([Required] int guideId, [Required] string hidden)
        {
            var serviceResponse = await _guidesService.ChangeGuideVisibility(guideId, hidden);

            return StatusCode(serviceResponse.StatusCode, serviceResponse.ToControllerResponse());
        }

        [DisableRequestSizeLimit]
        [HttpPut("part-guide/{id}")]
        public async Task<IActionResult> UpdatePartGuide([Required] int id, [FromForm] UpdatePartGuideDto updatedGuide)
        {
            var serviceResponse = await _guidesService.UpdatePartGuide(id, updatedGuide);

            return StatusCode(serviceResponse.StatusCode, serviceResponse.ToControllerResponse());
        }

        [HttpPut("switch")]
        public async Task<IActionResult> SwitchPartGuides([Required] int partGuideId1, [Required] int partGuideId2)
        {
            var serviceResponse = await _guidesService.SwitchPartGuides(partGuideId1, partGuideId2);

            return StatusCode(serviceResponse.StatusCode, serviceResponse.ToControllerResponse());
        }

        [HttpDelete("guide/{guideId}")]
        public async Task<IActionResult> RemoveGuide([Required] int guideId)
        {
            var serviceResponse = await _guidesService.RemoveGuide(guideId);

            return StatusCode(serviceResponse.StatusCode, serviceResponse.ToControllerResponse());
        }

        [HttpDelete("part-guide/{id}")]
        public async Task<IActionResult> RemovePartGuide([Required] int id)
        {
            var serviceResponse = await _guidesService.RemovePartGuide(id);

            return StatusCode(serviceResponse.StatusCode, serviceResponse.ToControllerResponse());
        }
    }
}