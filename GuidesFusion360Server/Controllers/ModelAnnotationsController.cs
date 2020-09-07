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
            var (serviceResponse, statusCode) = await _modelAnnotationsService.GetPublicModelAnnotations(guideId);

            return statusCode switch
            {
                401 => Unauthorized(serviceResponse),
                404 => NotFound(serviceResponse),
                _ => Ok(serviceResponse)
            };
        }

        [HttpGet("all/{guideId}")]
        public async Task<IActionResult> GetPrivateAnnotations([Required] int guideId)
        {
            var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!.Value);

            var (serviceResponse, statusCode) =
                await _modelAnnotationsService.GetPrivateModelAnnotations(guideId, userId);

            return statusCode switch
            {
                401 => Unauthorized(serviceResponse),
                404 => NotFound(serviceResponse),
                _ => Ok(serviceResponse)
            };
        }

        [HttpPost("new")]
        public async Task<IActionResult> AddModelAnnotation(AddModelAnnotationDto newAnnotation)
        {
            var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!.Value);

            var (serviceResponse, statusCode) =
                await _modelAnnotationsService.AddModelAnnotation(newAnnotation, userId);

            return statusCode switch
            {
                400 => BadRequest(serviceResponse),
                401 => Unauthorized(serviceResponse),
                404 => NotFound(serviceResponse),
                _ => Ok(serviceResponse)
            };
        }

        [HttpDelete("annotation/{annotationId}")]
        public async Task<IActionResult> RemoveAnnotation([Required] int annotationId)
        {
            var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!.Value);

            var (serviceResponse, statusCode) = await _modelAnnotationsService.RemoveAnnotation(annotationId, userId);

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