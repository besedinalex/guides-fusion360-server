using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GuidesFusion360Server.Dtos;
using GuidesFusion360Server.Models;

namespace GuidesFusion360Server.Services
{
    public interface IModelAnnotationsService
    {
        /// <summary>Requests public model annotations.</summary>
        /// <param name="guideId">Id of the guide.</param>
        /// <returns>Returns list of annotations and http code.</returns>
        public Task<Tuple<ServiceResponseModel<List<GetModelAnnotationsDto>>, int>> GetPublicModelAnnotations(
            int guideId);

        /// <summary>Requests private model annotations.</summary>
        /// <param name="guideId">Id of the guide.</param>
        /// <param name="userId">Id of the requester.</param>
        /// <returns>Returns list of annotations and http code.</returns>
        public Task<Tuple<ServiceResponseModel<List<GetModelAnnotationsDto>>, int>> GetPrivateModelAnnotations(
            int guideId, int userId);

        /// <summary>Requests to add annotation.</summary>
        /// <param name="newAnnotation">Annotation data.</param>
        /// <param name="userId">Id of the requester.</param>
        /// <returns>Returns id of the new annotation and http code.</returns>
        public Task<Tuple<ServiceResponseModel<int>, int>> AddModelAnnotation(AddModelAnnotationDto newAnnotation,
            int userId);

        /// <summary>Requests to delete annotation.</summary>
        /// <param name="annotationId">Id of the annotation.</param>
        /// <param name="userId">Id of the requester.</param>
        /// <returns>Returns http code.</returns>
        public Task<Tuple<ServiceResponseModel<int>, int>> RemoveAnnotation(int annotationId, int userId);
    }
}