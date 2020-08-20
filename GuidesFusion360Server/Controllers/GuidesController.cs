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

        [AllowAnonymous]
        [HttpGet("parts")]
        public async Task<IActionResult> GetAllPartGuides(int guideId)
        {
            return Ok(await _guidesService.GetAllPartGuideData(guideId));
        }

        [HttpPost("guide")]
        public async Task<IActionResult> CreateNewGuide(AddNewGuideDto newGuide)
        {
            var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            return Ok(await _guidesService.CreateNewGuide(userId, newGuide));
        }
    }
}
