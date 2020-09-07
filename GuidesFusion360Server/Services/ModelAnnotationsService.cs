using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GuidesFusion360Server.Data.Repositories;
using GuidesFusion360Server.Dtos;
using GuidesFusion360Server.Models;

namespace GuidesFusion360Server.Services
{
    public class ModelAnnotationsService : IModelAnnotationsService
    {
        private readonly IMapper _mapper;
        private readonly IModelAnnotationsRepository _modelAnnotationsRepository;
        private readonly IGuidesRepository _guidesRepository;
        private readonly IUsersRepository _usersRepository;

        public ModelAnnotationsService(IMapper mapper, IModelAnnotationsRepository modelAnnotationsRepository,
            IGuidesRepository guidesRepository, IUsersRepository usersRepository)
        {
            _mapper = mapper;
            _modelAnnotationsRepository = modelAnnotationsRepository;
            _guidesRepository = guidesRepository;
            _usersRepository = usersRepository;
        }

        public async Task<Tuple<ServiceResponseModel<List<GetModelAnnotationsDto>>, int>> GetPublicModelAnnotations(
            int guideId)
        {
            var (isPublic, accessResponse, statusCode) = await GuideIsPublic<List<GetModelAnnotationsDto>>(guideId);
            if (!isPublic)
            {
                return new Tuple<ServiceResponseModel<List<GetModelAnnotationsDto>>, int>(accessResponse, statusCode);
            }

            var annotations = await _modelAnnotationsRepository.GetAnnotations(guideId);

            var serviceResponse = new ServiceResponseModel<List<GetModelAnnotationsDto>>
            {
                Data = annotations.Select(x => _mapper.Map<GetModelAnnotationsDto>(x)).ToList()
            };

            return new Tuple<ServiceResponseModel<List<GetModelAnnotationsDto>>, int>(serviceResponse, 200);
        }

        public async Task<Tuple<ServiceResponseModel<List<GetModelAnnotationsDto>>, int>> GetPrivateModelAnnotations(
            int guideId, int userId)
        {
            var serviceResponse = new ServiceResponseModel<List<GetModelAnnotationsDto>>();

            var hasAccess = await _usersRepository.UserIsEditor(userId);
            if (!hasAccess)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "User access should be editor or admin.";
                return new Tuple<ServiceResponseModel<List<GetModelAnnotationsDto>>, int>(serviceResponse, 401);
            }

            var annotations = await _modelAnnotationsRepository.GetAnnotations(guideId);

            serviceResponse.Data = annotations.Select(x => _mapper.Map<GetModelAnnotationsDto>(x)).ToList();
            return new Tuple<ServiceResponseModel<List<GetModelAnnotationsDto>>, int>(serviceResponse, 200);
        }

        public async Task<Tuple<ServiceResponseModel<int>, int>> AddModelAnnotation(AddModelAnnotationDto newAnnotation,
            int userId)
        {
            var (isEditable, accessResponse, statusCode) = await GuideIsEditable<int>(userId, newAnnotation.GuideId);
            if (!isEditable)
            {
                return new Tuple<ServiceResponseModel<int>, int>(accessResponse, statusCode);
            }

            var annotation = _mapper.Map<ModelAnnotationModel>(newAnnotation);

            var annotationId = await _modelAnnotationsRepository.AddAnnotation(annotation);

            var serviceResponse = new ServiceResponseModel<int>
            {
                Data = annotationId,
                Message = "Annotation is added successfully."
            };
            return new Tuple<ServiceResponseModel<int>, int>(serviceResponse, 200);
        }

        public async Task<Tuple<ServiceResponseModel<int>, int>> RemoveAnnotation(int annotationId, int userId)
        {
            var annotation = await _modelAnnotationsRepository.GetAnnotation(annotationId);
            var (isEditable, accessResponse, statusCode) = await GuideIsEditable<int>(userId, annotation.GuideId);
            if (!isEditable)
            {
                return new Tuple<ServiceResponseModel<int>, int>(accessResponse, statusCode);
            }

            await _modelAnnotationsRepository.DeleteAnnotation(annotation);

            var serviceResponse = new ServiceResponseModel<int> {Message = "Annotation is deleted successfully."};
            return new Tuple<ServiceResponseModel<int>, int>(serviceResponse, 200);
        }

        /// <summary>Checks if guide is available for edit and if it exists at all.</summary>
        /// <param name="guideId">Id of the guide.</param>
        /// <typeparam name="T">Type of service response.</typeparam>
        /// <returns>Returns service response, http code of response and guide itself.</returns>
        private async Task<Tuple<bool, ServiceResponseModel<T>, int>> GuideIsPublic<T>(int guideId)
        {
            var serviceResponse = new ServiceResponseModel<T>();

            var guide = await _guidesRepository.GetGuide(guideId);

            if (guide == null)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "Guide with this id was not found.";
                return new Tuple<bool, ServiceResponseModel<T>, int>(false, serviceResponse, 404);
            }

            if (guide.Hidden == "true")
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "Guide with this id is not public.";
                return new Tuple<bool, ServiceResponseModel<T>, int>(false, serviceResponse, 401);
            }

            return new Tuple<bool, ServiceResponseModel<T>, int>(true, serviceResponse, 200);
        }

        /// <summary>Checks if guide is editable with current access level.</summary>
        /// <param name="userId">Id of the user who will edit the guide.</param>
        /// <param name="guideId">Id of the guide.</param>
        /// <typeparam name="T">Type of service response.</typeparam>
        /// <returns>Returns service response, http code of response and guide itself.</returns>
        private async Task<Tuple<bool, ServiceResponseModel<T>, int>> GuideIsEditable<T>(int userId, int guideId)
        {
            var serviceResponse = new ServiceResponseModel<T>();

            var isEditor = await _usersRepository.UserIsEditor(userId);
            if (!isEditor)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "User access should be editor or admin.";
                return new Tuple<bool, ServiceResponseModel<T>, int>(false, serviceResponse, 401);
            }

            var guide = await _guidesRepository.GetGuide(guideId);
            if (guide == null)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "Guide with this id was not found.";
                return new Tuple<bool, ServiceResponseModel<T>, int>(false, serviceResponse, 404);
            }

            if (guide.Hidden == "false")
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "Guide should be hidden in order to edit it.";
                return new Tuple<bool, ServiceResponseModel<T>, int>(false, serviceResponse, 400);
            }

            return new Tuple<bool, ServiceResponseModel<T>, int>(true, serviceResponse, 200);
        }
    }
}