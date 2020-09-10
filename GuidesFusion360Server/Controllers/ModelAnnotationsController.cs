using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using GuidesFusion360Server.Dtos.ModelAnnotations;
using GuidesFusion360Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GuidesFusion360Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("model-annotations")]
    public class ModelAnnotationsController : ControllerBase
    {
        private readonly IModelAnnotationsService _modelAnnotationsService;

        public ModelAnnotationsController(IModelAnnotationsService modelAnnotationsService)
        {
            _modelAnnotationsService = modelAnnotationsService;
        }

        [AllowAnonymous]
        [HttpGet("all-public/{guideId}")]
        public async Task<IActionResult> GetPublicAnnotations([Required] int guideId)
        {
            var serviceResponse = await _modelAnnotationsService.GetPublicModelAnnotations(guideId);

            return StatusCode(serviceResponse.StatusCode, serviceResponse.ToControllerResponse());
        }

        [HttpGet("all/{guideId}")]
        public async Task<IActionResult> GetPrivateAnnotations([Required] int guideId)
        {
            var serviceResponse = await _modelAnnotationsService.GetPrivateModelAnnotations(guideId);

            return StatusCode(serviceResponse.StatusCode, serviceResponse.ToControllerResponse());
        }

        [HttpPost("new")]
        public async Task<IActionResult> AddModelAnnotation(AddModelAnnotationDto newAnnotation)
        {
            var serviceResponse = await _modelAnnotationsService.AddModelAnnotation(newAnnotation);

            return StatusCode(serviceResponse.StatusCode, serviceResponse.ToControllerResponse());
        }

        [HttpDelete("annotation/{annotationId}")]
        public async Task<IActionResult> RemoveAnnotation([Required] int annotationId)
        {
            var serviceResponse = await _modelAnnotationsService.RemoveAnnotation(annotationId);

            return StatusCode(serviceResponse.StatusCode, serviceResponse.ToControllerResponse());
        }
    }
}