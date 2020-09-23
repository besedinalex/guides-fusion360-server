using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using GuidesFusion360Server.Data.Repositories;
using GuidesFusion360Server.Dtos.ModelAnnotations;
using GuidesFusion360Server.Models;
using Microsoft.AspNetCore.Http;

namespace GuidesFusion360Server.Services
{
    public class ModelAnnotationsService : IModelAnnotationsService
    {
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IModelAnnotationsRepository _modelAnnotationsRepository;
        private readonly IUsersRepository _usersRepository;
        private readonly IGuidesService _guidesService;

        private int GetUserId
        {
            get
            {
                try
                {
                    return int.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
                }
                catch
                {
                    return -1;
                }
            }
        }

        public ModelAnnotationsService(IMapper mapper, IHttpContextAccessor httpContextAccessor,
            IModelAnnotationsRepository modelAnnotationsRepository, IUsersRepository usersRepository,
            IGuidesService guidesService)
        {
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _modelAnnotationsRepository = modelAnnotationsRepository;
            _usersRepository = usersRepository;
            _guidesService = guidesService;
        }

        /// <inheritdoc />
        public async Task<ServiceResponse<List<GetModelAnnotationDto>>> GetPublicModelAnnotations(int guideId)
        {
            var (isPublic, accessResponse, guide) =
                await _guidesService.GuideIsPublic<List<GetModelAnnotationDto>>(guideId);
            if (!isPublic)
            {
                return accessResponse;
            }

            var annotations = await _modelAnnotationsRepository.GetAnnotations(guideId);

            return new ServiceResponse<List<GetModelAnnotationDto>>
            {
                Data = annotations.Select(x => _mapper.Map<GetModelAnnotationDto>(x)).ToList()
            };
        }

        /// <inheritdoc />
        public async Task<ServiceResponse<List<GetModelAnnotationDto>>> GetPrivateModelAnnotations(int guideId)
        {
            var serviceResponse = new ServiceResponse<List<GetModelAnnotationDto>>();

            var hasAccess = await _usersRepository.UserIsEditor(GetUserId);
            if (!hasAccess)
            {
                serviceResponse.StatusCode = 401;
                serviceResponse.Message = "User access should be editor or admin.";
                serviceResponse.MessageRu = "Пользователь должен быть Редактором или Администратором.";
                return serviceResponse;
            }

            var annotations = await _modelAnnotationsRepository.GetAnnotations(guideId);

            serviceResponse.Data = annotations.Select(x => _mapper.Map<GetModelAnnotationDto>(x)).ToList();
            return serviceResponse;
        }

        /// <inheritdoc />
        public async Task<ServiceResponse<int>> AddModelAnnotation(AddModelAnnotationDto newAnnotation)
        {
            var (isEditable, accessResponse, guide) =
                await _guidesService.GuideIsEditable<int>((int) newAnnotation.GuideId!);
            if (!isEditable)
            {
                return accessResponse;
            }

            var annotation = _mapper.Map<ModelAnnotation>(newAnnotation);

            var annotationId = await _modelAnnotationsRepository.AddAnnotation(annotation);

            return new ServiceResponse<int>
            {
                StatusCode = 201, Data = annotationId, Message = "Annotation is successfully created.",
                MessageRu = "Аннотация успешно создана."
            };
        }

        /// <inheritdoc />
        public async Task<ServiceResponse<int>> RemoveAnnotation(int annotationId)
        {
            var annotation = await _modelAnnotationsRepository.GetAnnotation(annotationId);

            if (annotation == null)
            {
                return new ServiceResponse<int>
                    {StatusCode = 404, Message = "Annotation is not found.", MessageRu = "Аннотация не найдена."};
            }

            var (isEditable, accessResponse, guide) = await _guidesService.GuideIsEditable<int>(annotation.GuideId);
            if (!isEditable)
            {
                return accessResponse;
            }

            await _modelAnnotationsRepository.DeleteAnnotation(annotation);

            return new ServiceResponse<int>
                {Message = "Annotation is deleted successfully.", MessageRu = "Аннотация успешно удалена."};
        }
    }
}