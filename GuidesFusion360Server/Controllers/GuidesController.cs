using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using GuidesFusion360Server.Dtos;
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
        public async Task<IActionResult> GetAllGuides()
        {
            return Ok(await _guidesService.GetAllGuides());
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
        [HttpGet("preview/{guideId}")]
        public async Task<IActionResult> GetGuidePreview([Required]int guideId)
        {
            var (serviceResponse, statusCode) = await _guidesService.GetPublicGuidePreview(guideId);
            switch (statusCode)
            {
                case 404:
                    return NotFound(serviceResponse);
                case 401:
                    try
                    {
                        var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!
                            .Value);
                        serviceResponse = await _guidesService.GetPrivateGuidePreview(guideId, userId);
                        if (!serviceResponse.Success)
                        {
                            return Unauthorized(serviceResponse);
                        }

                        return serviceResponse.Data;
                    }
                    catch
                    {
                        return BadRequest(serviceResponse);
                    }
                default: // 200
                    return serviceResponse.Data;
            }
        }

        [AllowAnonymous]
        [HttpGet("parts/{guideId}")]
        public async Task<IActionResult> GetAllPartGuides([Required]int guideId)
        {
            var (serviceResponse, statusCode) = await _guidesService.GetAllPublicPartGuides(guideId);
            switch (statusCode)
            {
                case 404:
                    return NotFound(serviceResponse);
                case 401:
                    try
                    {
                        var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!
                            .Value);
                        serviceResponse = await _guidesService.GetAllPrivatePartGuides(guideId, userId);
                        if (!serviceResponse.Success)
                        {
                            return Unauthorized(serviceResponse);
                        }

                        return Ok(serviceResponse);
                    }
                    catch
                    {
                        return BadRequest(serviceResponse);
                    }
                default: // 200
                    return Ok(serviceResponse);
            }
        }
        
        [HttpPost("guide")]
        public async Task<IActionResult> CreateNewGuide([FromForm] AddNewGuideDto newGuide)
        {
            var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!.Value);

            var (serviceResponse, statusCode) = await _guidesService.CreateNewGuide(userId, newGuide);

            return statusCode switch
            {
                400 => BadRequest(serviceResponse),
                401 => Unauthorized(serviceResponse),
                _ => Ok(serviceResponse)
            };
        }

        [DisableRequestSizeLimit]
        [HttpPost("part-guide")]
        public async Task<IActionResult> CreateNewPartGuide([FromForm] AddNewPartGuideDto newGuide)
        {
            var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!.Value);

            var (serviceResponse, statusCode) = await _guidesService.CreateNewPartGuide(userId, newGuide);

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
        public async Task<IActionResult> UploadModel([FromForm] AddNewGuideModelDto newModel)
        {
            var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!.Value);
            
            var (serviceResponse, statusCode) = await _guidesService.UploadModel(userId, newModel);

            return statusCode switch
            {
                400 => BadRequest(serviceResponse),
                401 => Unauthorized(serviceResponse),
                404 => NotFound(serviceResponse),
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
        public async Task<IActionResult> UpdatePartGuide([Required]int id, [FromForm] UpdatePartGuideDto updatedGuide)
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
    }
}
