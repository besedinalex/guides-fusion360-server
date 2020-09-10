using System.Collections.Generic;
using System.Threading.Tasks;
using GuidesFusion360Server.Dtos.ModelAnnotations;
using GuidesFusion360Server.Models;

namespace GuidesFusion360Server.Services
{
    public interface IModelAnnotationsService
    {
        /// <summary>Requests public model annotations.</summary>
        /// <param name="guideId">Id of the guide.</param>
        /// <returns>Returns list of annotations.</returns>
        public Task<ServiceResponse<List<GetModelAnnotationDto>>> GetPublicModelAnnotations(int guideId);

        /// <summary>Requests private model annotations.</summary>
        /// <param name="guideId">Id of the guide.</param>
        /// <returns>Returns list of annotations.</returns>
        public Task<ServiceResponse<List<GetModelAnnotationDto>>> GetPrivateModelAnnotations(int guideId);

        /// <summary>Requests to add annotation.</summary>
        /// <param name="newAnnotation">Annotation data.</param>
        /// <returns>Returns id of the new annotation.</returns>
        public Task<ServiceResponse<int>> AddModelAnnotation(AddModelAnnotationDto newAnnotation);

        /// <summary>Requests to delete annotation.</summary>
        /// <param name="annotationId">Id of the annotation.</param>
        public Task<ServiceResponse<int>> RemoveAnnotation(int annotationId);
    }
}