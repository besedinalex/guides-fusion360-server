using System;
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
        public async Task<IActionResult> GetGuidePreview(int guideId)
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
        public async Task<IActionResult> GetAllPartGuides(int guideId)
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

                        return BadRequest(serviceResponse);
                    }
                    catch
                    {
                        return Ok(serviceResponse);
                    }
            }

            return Ok(await _guidesService.GetAllPublicPartGuides(guideId));
        }

        [HttpPost("guide")]
        public async Task<IActionResult> CreateNewGuide([FromForm] AddNewGuideDto newGuide)
        {
            var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!.Value);

            var serviceResponse = await _guidesService.CreateNewGuide(userId, newGuide);

            if (!serviceResponse.Success)
            {
                return BadRequest(serviceResponse);
            }

            return Ok(serviceResponse);
        }
    }
}
