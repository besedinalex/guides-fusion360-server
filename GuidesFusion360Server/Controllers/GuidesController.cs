using System.Threading.Tasks;
using GuidesFusion360Server.Dtos;
using GuidesFusion360Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace GuidesFusion360Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GuidesController : ControllerBase
    {
        private readonly IGuidesService _guidesService;
        
        public GuidesController(IGuidesService guidesService)
        {
            _guidesService = guidesService;
        }
        
        [HttpGet("all")]
        public async Task<IActionResult> GetAllGuides()
        {
            return Ok(await _guidesService.GetAllGuides());
        }

        [HttpGet("parts")]
        public async Task<IActionResult> GetAllPartGuides(int guideId)
        {
            return Ok(await _guidesService.GetAllPartGuideData(guideId));
        }

        [HttpPost("guide")]
        public async Task<IActionResult> CreateNewGuide(AddNewGuideDto newGuide)
        {
            return Ok(await _guidesService.CreateNewGuide(0, newGuide));
        }
    }
}
