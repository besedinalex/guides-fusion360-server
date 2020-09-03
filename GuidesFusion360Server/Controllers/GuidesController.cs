using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using GuidesFusion360Server.Dtos;
using GuidesFusion360Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
            return Ok(await _guidesService.GetAllPublicGuides());
        }

        [HttpGet("all-hidden")]
        public async Task<IActionResult> GetAllHiddenGuides()
        {
            var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!.Value);
            var serviceResponse = await _guidesService.GetAllHiddenGuides(userId);

            if (!serviceResponse.Success)
            {
                return Unauthorized(serviceResponse);
            }

            return Ok(serviceResponse);
        }

        [AllowAnonymous]
        [HttpGet("parts-public/{guideId}")]
        public async Task<IActionResult> GetPublicPartGuides([Required] int guideId)
        {
            var (serviceResponse, statusCode) = await _guidesService.GetPartGuides(guideId, -1);

            return statusCode switch
            {
                401 => Unauthorized(serviceResponse),
                404 => NotFound(serviceResponse),
                _ => Ok(serviceResponse)
            };
        }

        [HttpGet("parts/{guideId}")]
        public async Task<IActionResult> GetPrivatePartGuides([Required] int guideId)
        {
            var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!.Value);

            var (serviceResponse, statusCode) = await _guidesService.GetPartGuides(guideId, userId);

            return statusCode switch
            {
                401 => Unauthorized(serviceResponse),
                404 => NotFound(serviceResponse),
                _ => Ok(serviceResponse)
            };
        }

        [AllowAnonymous]
        [HttpGet("file-public/{guideId}")]
        public async Task<IActionResult> GetPublicGuideFile([Required] int guideId, [Required] string filename)
        {
            var (serviceResponse, statusCode) = await _guidesService.GetGuideFile(guideId, filename, -1);

            return statusCode switch
            {
                401 => Unauthorized(serviceResponse),
                404 => NotFound(serviceResponse),
                _ => serviceResponse.Data
            };
        }

        [HttpGet("file/{guideId}")]
        public async Task<IActionResult> GetPrivateGuideFile([Required] int guideId, [Required] string filename)
        {
            var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!.Value);

            var (serviceResponse, statusCode) = await _guidesService.GetGuideFile(guideId, filename, userId);

            return statusCode switch
            {
                401 => Unauthorized(serviceResponse),
                404 => NotFound(serviceResponse),
                _ => serviceResponse.Data
            };
        }

        [HttpPost("guide")]
        public async Task<IActionResult> CreateNewGuide([FromForm] AddGuideDto guide)
        {
            var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!.Value);

            var (serviceResponse, statusCode) = await _guidesService.CreateGuide(userId, guide);

            return statusCode switch
            {
                400 => BadRequest(serviceResponse),
                401 => Unauthorized(serviceResponse),
                _ => Ok(serviceResponse)
            };
        }

        [DisableRequestSizeLimit]
        [HttpPost("part-guide")]
        public async Task<IActionResult> CreateNewPartGuide([FromForm] AddPartGuideDto guide)
        {
            var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!.Value);

            var (serviceResponse, statusCode) = await _guidesService.CreatePartGuide(userId, guide);

            return statusCode switch
            {
                400 => BadRequest(serviceResponse),
                401 => Unauthorized(serviceResponse),
                404 => NotFound(serviceResponse),
                _ => Ok(serviceResponse)
            };
        }

        [DisableRequestSizeLimit]
        [HttpPost("model")]
        public async Task<IActionResult> UploadModel([FromForm] AddGuideModelDto model)
        {
            var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!.Value);

            var (serviceResponse, statusCode) = await _guidesService.UploadModel(userId, model);

            return statusCode switch
            {
                400 => BadRequest(serviceResponse),
                401 => Unauthorized(serviceResponse),
                404 => NotFound(serviceResponse),
                500 => StatusCode(StatusCodes.Status500InternalServerError, serviceResponse),
                _ => Ok(serviceResponse)
            };
        }

        [HttpPut("hidden/{guideId}")]
        public async Task<IActionResult> ChangeGuideVisibility([Required] int guideId, [Required] string hidden)
        {
            var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!.Value);

            var (serviceResponse, statusCode) = await _guidesService.ChangeGuideVisibility(userId, guideId, hidden);

            return statusCode switch
            {
                400 => BadRequest(serviceResponse),
                401 => Unauthorized(serviceResponse),
                404 => NotFound(serviceResponse),
                _ => Ok(serviceResponse)
            };
        }

        [DisableRequestSizeLimit]
        [HttpPut("part-guide/{id}")]
        public async Task<IActionResult> UpdatePartGuide([Required] int id, [FromForm] UpdatePartGuideDto updatedGuide)
        {
            var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!.Value);

            var (serviceResponse, statusCode) = await _guidesService.UpdatePartGuide(userId, id, updatedGuide);

            return statusCode switch
            {
                400 => BadRequest(serviceResponse),
                401 => Unauthorized(serviceResponse),
                404 => NotFound(serviceResponse),
                _ => Ok(serviceResponse)
            };
        }

        [HttpPut("switch")]
        public async Task<IActionResult> SwitchPartGuides([Required] int partGuideId1, [Required] int partGuideId2)
        {
            var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!.Value);

            var (serviceResponse, statusCode) =
                await _guidesService.SwitchPartGuides(userId, partGuideId1, partGuideId2);

            return statusCode switch
            {
                400 => BadRequest(serviceResponse),
                401 => Unauthorized(serviceResponse),
                404 => NotFound(serviceResponse),
                _ => Ok(serviceResponse)
            };
        }

        [HttpDelete("guide/{guideId}")]
        public async Task<IActionResult> RemoveGuide([Required] int guideId)
        {
            var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!.Value);

            var (serviceResponse, statusCode) = await _guidesService.RemoveGuide(userId, guideId);

            return statusCode switch
            {
                400 => BadRequest(serviceResponse),
                401 => Unauthorized(serviceResponse),
                404 => NotFound(serviceResponse),
                _ => Ok(serviceResponse)
            };
        }

        [HttpDelete("part-guide/{id}")]
        public async Task<IActionResult> RemovePartGuide([Required] int id)
        {
            var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!.Value);

            var (serviceResponse, statusCode) = await _guidesService.RemovePartGuide(userId, id);

            return statusCode switch
            {
                400 => BadRequest(serviceResponse),
                401 => Unauthorized(serviceResponse),
                404 => NotFound(serviceResponse),
                _ => Ok(serviceResponse)
            };
        }
    }
}