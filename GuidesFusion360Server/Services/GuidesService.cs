using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using GuidesFusion360Server.Data;
using GuidesFusion360Server.Data.Repositories;
using GuidesFusion360Server.Dtos.Guides;
using GuidesFusion360Server.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace GuidesFusion360Server.Services
{
    /// <inheritdoc />
    public class GuidesService : IGuidesService
    {
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IFileManager _fileManager;
        private readonly IGuidesRepository _guidesRepository;
        private readonly IUsersRepository _usersRepository;
        private readonly IModelAnnotationsRepository _modelAnnotationsRepository;
        private readonly IHttpClientFactory _clientFactory;

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

        public GuidesService(IMapper mapper, IHttpContextAccessor httpContextAccessor, IFileManager fileManager,
            IGuidesRepository guidesRepository, IUsersRepository usersRepository,
            IModelAnnotationsRepository modelAnnotationsRepository, IHttpClientFactory clientFactory)
        {
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _fileManager = fileManager;
            _guidesRepository = guidesRepository;
            _usersRepository = usersRepository;
            _modelAnnotationsRepository = modelAnnotationsRepository;
            _clientFactory = clientFactory;
        }

        /// <inheritdoc />
        public async Task<ServiceResponse<List<GetGuideDto>>> GetAllPublicGuides()
        {
            var serviceResponse = new ServiceResponse<List<GetGuideDto>>();

            var guides = await _guidesRepository.GetAllPublicGuides();

            serviceResponse.Data = guides.Select(c => _mapper.Map<GetGuideDto>(c)).ToList();
            return serviceResponse;
        }

        /// <inheritdoc />
        public async Task<ServiceResponse<List<GetGuideDto>>> GetAllHiddenGuides()
        {
            var serviceResponse = new ServiceResponse<List<GetGuideDto>>();

            var hasAccess = await _usersRepository.UserIsEditor(GetUserId);
            if (!hasAccess)
            {
                serviceResponse.StatusCode = 401;
                serviceResponse.Message = "User access should be editor or admin.";
                serviceResponse.MessageRu = "Пользователь должен быть Редактором или Администратором.";
                return serviceResponse;
            }

            var guides = await _guidesRepository.GetAllHiddenGuides();

            serviceResponse.Data = guides.Select(c => _mapper.Map<GetGuideDto>(c)).ToList();
            return serviceResponse;
        }

        /// <inheritdoc />
        public async Task<ServiceResponse<List<GetPartGuideDto>>> GetPartGuides(int guideId)
        {
            var serviceResponse = new ServiceResponse<List<GetPartGuideDto>>();

            var (isPublic, accessResponse, guide) = await GuideIsPublic<List<GetPartGuideDto>>(guideId);

            if (!isPublic)
            {
                if (GetUserId == -1)
                {
                    return accessResponse;
                }

                if (guide == null)
                {
                    return accessResponse;
                }

                var hasAccess = await _usersRepository.UserIsEditor(GetUserId);
                if (!hasAccess)
                {
                    serviceResponse.StatusCode = 401;
                    serviceResponse.Message = "User access should be editor or admin.";
                    serviceResponse.MessageRu = "Пользователь должен быть Редактором или Администратором.";
                    return serviceResponse;
                }
            }

            var guides = await _guidesRepository.GetPartGuides(guideId);

            serviceResponse.Data = guides.Select(c => _mapper.Map<GetPartGuideDto>(c)).ToList();
            return serviceResponse;
        }

        /// <inheritdoc />
        public async Task<ServiceResponse<FileContentResult>> GetGuideFile(int guideId, string filename)
        {
            var serviceResponse = new ServiceResponse<FileContentResult>();

            var (isPublic, accessResponse, guide) = await GuideIsPublic<FileContentResult>(guideId);

            if (!isPublic)
            {
                if (GetUserId == -1)
                {
                    return accessResponse;
                }

                if (guide == null)
                {
                    return accessResponse;
                }

                var hasAccess = await _usersRepository.UserIsEditor(GetUserId);

                if (!hasAccess)
                {
                    serviceResponse.StatusCode = 401;
                    serviceResponse.Message = "User access should be editor or admin.";
                    serviceResponse.MessageRu = "Пользователь должен быть Редактором или Администратором.";
                    return serviceResponse;
                }
            }

            if (!_fileManager.FileExists(guideId, filename))
            {
                serviceResponse.StatusCode = 404;
                serviceResponse.Message = "File is not found.";
                serviceResponse.MessageRu = "Файл не найден.";
                return serviceResponse;
            }

            var file = await _fileManager.GetFile(guideId, filename);
            var extension = System.IO.Path.GetExtension(filename).ToLower();
            var mimeType = extension switch
            {
                ".png" => "image/png",
                ".pdf" => "application/pdf",
                _ => "application/octet-stream"
            };

            serviceResponse.Data = new FileContentResult(file, mimeType);
            return serviceResponse;
        }

        /// <inheritdoc />
        public async Task<ServiceResponse<GetGuideOwnerDto>> GetGuideOwner(int guideId)
        {
            var serviceResponse = new ServiceResponse<GetGuideOwnerDto>();

            var hasAccess = await _usersRepository.UserIsEditor(GetUserId);
            if (!hasAccess)
            {
                serviceResponse.StatusCode = 401;
                serviceResponse.Message = "User access should be editor or admin.";
                serviceResponse.MessageRu = "Пользователь должен быть Редактором или Администратором.";
                return serviceResponse;
            }

            var guide = await _guidesRepository.GetGuide(guideId);
            if (guide == null)
            {
                serviceResponse.StatusCode = 404;
                serviceResponse.Message = "Guide is not found.";
                serviceResponse.MessageRu = "Гайд не найден.";
                return serviceResponse;
            }

            var owner = await _usersRepository.GetUser(guide.OwnerId);

            serviceResponse.Data = _mapper.Map<GetGuideOwnerDto>(owner);
            return serviceResponse;
        }

        /// <inheritdoc />
        public async Task<ServiceResponse<int>> CreateGuide(AddGuideDto newGuide)
        {
            var serviceResponse = new ServiceResponse<int>();
            var userId = GetUserId;

            var hasAccess = await _usersRepository.UserIsEditor(userId);
            if (!hasAccess)
            {
                serviceResponse.StatusCode = 401;
                serviceResponse.Message = "User access should be editor or admin.";
                serviceResponse.MessageRu = "Пользователь должен быть Редактором или Администратором.";
                return serviceResponse;
            }

            if (newGuide.Image.ContentType != "image/png")
            {
                serviceResponse.StatusCode = 400;
                serviceResponse.Message = "File must be PNG image.";
                serviceResponse.MessageRu = "Файл должен являться PNG изображением.";
                return serviceResponse;
            }

            var guide = _mapper.Map<Guide>(newGuide);
            guide.OwnerId = userId;
            guide.Hidden = "true";

            var guideId = await _guidesRepository.CreateGuide(guide);
            await _fileManager.SaveFile(guideId, "preview.png", newGuide.Image);

            serviceResponse.StatusCode = 201;
            serviceResponse.Data = guideId;
            serviceResponse.Message = "Guide is successfully created.";
            serviceResponse.MessageRu = "Гайд успешно создан.";
            return serviceResponse;
        }

        /// <inheritdoc />
        public async Task<ServiceResponse<int>> CreatePartGuide(AddPartGuideDto newPartGuide)
        {
            var (isEditable, accessResponse, guide) = await GuideIsEditable<int>(newPartGuide.GuideId);
            if (!isEditable)
            {
                return accessResponse;
            }

            var serviceResponse = new ServiceResponse<int>();

            var content = newPartGuide.Content;
            var file = newPartGuide.File;

            if (file == null && content == null || file != null && content != null)
            {
                serviceResponse.StatusCode = 400;
                serviceResponse.Message =
                    "You should provide PDF or ZIP in file field or YouTube link in content field.";
                serviceResponse.MessageRu =
                    "В поле content необходимо ввести ссылку на YouTube видео или в поле file прикрепить PDF или ZIP файл.";
                return serviceResponse;
            }

            var partGuide = _mapper.Map<PartGuide>(newPartGuide);

            if (content != null && !Uri.IsWellFormedUriString(content, UriKind.Absolute))
            {
                serviceResponse.StatusCode = 400;
                serviceResponse.Message = "You should provide valid URL in link field.";
                serviceResponse.MessageRu = "В поле link необходимо указать валидный URL адрес.";
                return serviceResponse;
            }

            if (file != null)
            {
                var isPdf = file.ContentType == "application/pdf";
                var isZip = file.ContentType == "application/zip" || file.ContentType == "application/x-zip-compressed";
                if (!isPdf && !isZip)
                {
                    serviceResponse.StatusCode = 400;
                    serviceResponse.Message = "You should provide PDF or ZIP file.";
                    serviceResponse.MessageRu = "Необходимо отправить PDF или ZIP файл.";
                    return serviceResponse;
                }

                if (_fileManager.FileExists(partGuide.GuideId, file.FileName))
                {
                    serviceResponse.StatusCode = 400;
                    serviceResponse.Message = "File with this name already exists in this guide.";
                    serviceResponse.MessageRu = "Файл с таким именем уже существует.";
                    return serviceResponse;
                }

                partGuide.Content = file.FileName;
                await _fileManager.SaveFile(partGuide.GuideId, file.FileName, file);
            }

            var partGuideId = await _guidesRepository.CreatePartGuide(partGuide);

            serviceResponse.StatusCode = 201;
            serviceResponse.Data = partGuideId;
            serviceResponse.Message = "Part guide is successfully created.";
            serviceResponse.MessageRu = "Гайд детали успешно создан.";
            return serviceResponse;
        }

        /// <inheritdoc />
        public async Task<ServiceResponse<int>> UploadModel(AddGuideModelDto model)
        {
            var (isEditable, accessResponse, guide) = await GuideIsEditable<int>(model.GuideId);
            if (!isEditable)
            {
                return accessResponse;
            }

            using var formData = new MultipartFormDataContent();

            // 'File' field
            var bodyFile = new StreamContent(model.File.OpenReadStream());
            bodyFile.Headers.Add("Content-Type", "application/octet-stream");
            bodyFile.Headers.Add("Content-Disposition", "form-data; name=\"file\"; filename=\"model.stp\"");
            formData.Add(bodyFile, "file");

            // 'From' field
            var extFrom = new StringContent("stp");
            extFrom.Headers.Add("Content-Disposition", "form-data; name=\"from\"");
            formData.Add(extFrom, "from");

            // 'To' field
            var extTo = new StringContent("glb");
            extTo.Headers.Add("Content-Disposition", "form-data; name=\"to\"");
            formData.Add(extTo, "to");

            var serviceResponse = new ServiceResponse<int>();

            try
            {
                // MPU Cloud Exchanger
                using var client = _clientFactory.CreateClient("converter");
                var response = await client.PostAsync("/model", formData);
                response.EnsureSuccessStatusCode();
                var data = JObject.Parse(await response.Content.ReadAsStringAsync());
                var glbModel = Convert.FromBase64String(data["output"].ToString());

                await _fileManager.SaveFile(model.GuideId, "model.glb", glbModel);

                serviceResponse.StatusCode = 201;
                serviceResponse.Data = model.GuideId;
                serviceResponse.Message = "Model was converted to and saved as glb successfully.";
                serviceResponse.MessageRu = "Модель была успешно конвертирована в glb и сохранена.";
                return serviceResponse;
            }
            catch
            {
                serviceResponse.StatusCode = 500;
                serviceResponse.Message = "Unable to convert the model.";
                serviceResponse.MessageRu = "Не удалось конвертировать модель.";
                return serviceResponse;
            }
        }

        /// <inheritdoc />
        public async Task<ServiceResponse<int>> ChangeGuideVisibility(int guideId, string hidden)
        {
            var (isEditable, accessResponse, guide) = await GuideIsEditable<int>(guideId, true);
            if (!isEditable)
            {
                return accessResponse;
            }

            var serviceResponse = new ServiceResponse<int>();

            hidden = hidden.ToLower();

            if (hidden != "true" && hidden != "false")
            {
                serviceResponse.StatusCode = 400;
                serviceResponse.Message = "Hidden field should be true or false.";
                serviceResponse.MessageRu = "В поле hidden необходимо указать true или false.";
                return serviceResponse;
            }

            guide.Hidden = hidden;

            await _guidesRepository.UpdateGuide(guide);

            serviceResponse.Data = guideId;
            serviceResponse.Message = hidden == "true" ? "Guide is now hidden." : "Guide is now public.";
            serviceResponse.MessageRu = hidden == "true" ? "Гайд был скрыт." : "Гайд был опубликован.";
            return serviceResponse;
        }

        /// <inheritdoc />
        public async Task<ServiceResponse<int>> UpdatePartGuide(int partGuideId, UpdatePartGuideDto updatedGuide)
        {
            var (isEditable, accessResponse, partGuide) = await PartGuideIsEditable<int>(partGuideId);
            if (!isEditable)
            {
                return accessResponse;
            }

            var serviceResponse = new ServiceResponse<int>();

            var name = updatedGuide.Name;
            var content = updatedGuide.Content;
            var file = updatedGuide.File;

            if (file != null && content != null)
            {
                serviceResponse.StatusCode = 400;
                serviceResponse.Message = "You cannot provide file and content at the same time.";
                serviceResponse.MessageRu = "Вы не можете заполнить поля file и content одновременно.";
                return serviceResponse;
            }

            if (content != null)
            {
                if (!Uri.IsWellFormedUriString(content, UriKind.Absolute))
                {
                    serviceResponse.StatusCode = 400;
                    serviceResponse.Message = "You should provide valid URL in content field.";
                    serviceResponse.MessageRu = "Вы должны указать валидный URL в поле content.";
                    return serviceResponse;
                }

                if (_fileManager.FileExists(partGuide.GuideId, partGuide.Content))
                {
                    _fileManager.DeleteFile(partGuide.GuideId, partGuide.Content);
                }

                partGuide.Content = content;
            }

            if (file != null)
            {
                var isPdf = file.ContentType == "application/pdf";
                var isZip = file.ContentType == "application/zip" || file.ContentType == "application/x-zip-compressed";
                if (!isPdf && !isZip)
                {
                    serviceResponse.StatusCode = 400;
                    serviceResponse.Message = "You should provide PDF or ZIP file or no files at all.";
                    serviceResponse.MessageRu = "Вы должны загрузить PDF или ZIP файл или не загружать файл вовсе.";
                    return serviceResponse;
                }

                if (_fileManager.FileExists(partGuide.GuideId, file.FileName))
                {
                    if (file.FileName != partGuide.Content)
                    {
                        serviceResponse.StatusCode = 400;
                        serviceResponse.Message = "File with this name already exists in this guide.";
                        serviceResponse.MessageRu = "Файл с таким именем уже существует в данном гайде.";
                        return serviceResponse;
                    }

                    await _fileManager.SaveFile(partGuide.GuideId, file.FileName, file);
                }

                if (_fileManager.FileExists(partGuide.GuideId, partGuide.Content))
                {
                    _fileManager.DeleteFile(partGuide.GuideId, partGuide.Content);
                }

                partGuide.Content = file.FileName;
                await _fileManager.SaveFile(partGuide.GuideId, file.FileName, file);
            }

            if (name != null)
            {
                partGuide.Name = name;
            }

            await _guidesRepository.UpdatePartGuide(partGuide);

            serviceResponse.Data = partGuide.Id;
            serviceResponse.Message = "Part guide is successfully updated.";
            serviceResponse.MessageRu = "Гайд детали успешно обновлен.";
            return serviceResponse;
        }

        /// <inheritdoc />
        public async Task<ServiceResponse<int>> SwitchPartGuides(int partGuideId1, int partGuideId2)
        {
            var (isEditable, accessResponse, partGuide1) = await PartGuideIsEditable<int>(partGuideId1);
            if (!isEditable)
            {
                return accessResponse;
            }

            var serviceResponse = new ServiceResponse<int>();

            var partGuide2 = await _guidesRepository.GetPartGuide(partGuideId2);

            if (partGuide2 == null)
            {
                serviceResponse.StatusCode = 404;
                serviceResponse.Message = "Part guide with this id was not found";
                serviceResponse.MessageRu = "Гайд детали с таким id не был найден.";
                return serviceResponse;
            }

            if (partGuide1.GuideId != partGuide2.GuideId)
            {
                serviceResponse.StatusCode = 400;
                serviceResponse.Message = "Part guides should be from one guide.";
                serviceResponse.MessageRu = "Гайды детали должны принадлежать одному гайду.";
                return serviceResponse;
            }

            var partGuide1SortKey = partGuide1.SortKey;
            partGuide1.SortKey = partGuide2.SortKey;
            partGuide2.SortKey = partGuide1SortKey;

            await _guidesRepository.UpdatePartGuides(new List<PartGuide> {partGuide1, partGuide2});

            serviceResponse.Message = "Part guides are switched successfully.";
            serviceResponse.MessageRu = "Гайды детали успешно поменялись местами.";
            return serviceResponse;
        }

        /// <inheritdoc />
        public async Task<ServiceResponse<int>> RemoveGuide(int guideId)
        {
            var (isEditable, accessResponse, guide) = await GuideIsEditable<int>(guideId, true);
            if (!isEditable)
            {
                return accessResponse;
            }

            var serviceResponse = new ServiceResponse<int>();

            var guideModelAnnotations = await _modelAnnotationsRepository.GetAnnotations(guideId);

            _fileManager.DeleteFolder(guideId);
            await _modelAnnotationsRepository.DeleteAnnotations(guideModelAnnotations);
            await _guidesRepository.RemoveGuide(guide);

            serviceResponse.Message = "Guide is deleted successfully.";
            serviceResponse.MessageRu = "Гайд удален успешно.";
            return serviceResponse;
        }

        /// <inheritdoc />
        public async Task<ServiceResponse<int>> RemovePartGuide(int partGuideId)
        {
            var (isEditable, accessResponse, partGuide) = await PartGuideIsEditable<int>(partGuideId);
            if (!isEditable)
            {
                return accessResponse;
            }

            var serviceResponse = new ServiceResponse<int>();

            if (_fileManager.FileExists(partGuide.GuideId, partGuide.Content))
                _fileManager.DeleteFile(partGuide.GuideId, partGuide.Content);
            await _guidesRepository.RemovePartGuide(partGuide);

            serviceResponse.Message = "Part guide is deleted successfully.";
            serviceResponse.MessageRu = "Гайд детали удален успешно.";
            return serviceResponse;
        }

        /// <inheritdoc />
        public async Task<Tuple<bool, ServiceResponse<T>, Guide>> GuideIsPublic<T>(int guideId)
        {
            var serviceResponse = new ServiceResponse<T>();

            var guide = await _guidesRepository.GetGuide(guideId);

            if (guide == null)
            {
                serviceResponse.StatusCode = 404;
                serviceResponse.Message = "Guide with this id was not found.";
                serviceResponse.MessageRu = "Гайд с таким id не найден.";
                return new Tuple<bool, ServiceResponse<T>, Guide>(false, serviceResponse, null);
            }

            if (guide.Hidden == "true")
            {
                serviceResponse.StatusCode = 401;
                serviceResponse.Message = "Guide with this id is not public.";
                serviceResponse.MessageRu = "Гайд с таким id скрыт.";
                return new Tuple<bool, ServiceResponse<T>, Guide>(false, serviceResponse, guide);
            }

            return new Tuple<bool, ServiceResponse<T>, Guide>(true, serviceResponse, guide);
        }

        /// <inheritdoc />
        public async Task<Tuple<bool, ServiceResponse<T>, Guide>> GuideIsEditable<T>(int guideId,
            bool requiresAdminAccess = false)
        {
            var serviceResponse = new ServiceResponse<T>();

            var guide = await _guidesRepository.GetGuide(guideId);

            if (guide == null)
            {
                serviceResponse.StatusCode = 404;
                serviceResponse.Message = "Guide with this id was not found.";
                serviceResponse.MessageRu = "Гайд с таким id не найден.";
                return new Tuple<bool, ServiceResponse<T>, Guide>(false, serviceResponse, null);
            }

            var hasAccess = requiresAdminAccess
                ? await _usersRepository.UserIsAdmin(GetUserId)
                : await _usersRepository.UserIsEditor(GetUserId);
            if (!hasAccess)
            {
                serviceResponse.StatusCode = 401;
                serviceResponse.Message =
                    $"User access should be {(requiresAdminAccess ? "admin" : "editor or admin")}.";
                serviceResponse.MessageRu =
                    $"Пользователь должен быть {(requiresAdminAccess ? "Администратором" : "Редактором или Администратором")}.";
                return new Tuple<bool, ServiceResponse<T>, Guide>(false, serviceResponse, guide);
            }

            if (!requiresAdminAccess && guide.Hidden == "false")
            {
                serviceResponse.StatusCode = 400;
                serviceResponse.Message = "Guide should be hidden in order to edit it.";
                serviceResponse.MessageRu = "Чтобы редактировать гайд, его нужно скрыть.";
                return new Tuple<bool, ServiceResponse<T>, Guide>(false, serviceResponse, guide);
            }

            return new Tuple<bool, ServiceResponse<T>, Guide>(true, serviceResponse, guide);
        }

        /// <summary>Checks if part guide is editable with current access level.</summary>
        /// <param name="partGuideId">Id of the part guide.</param>
        /// <param name="requiresAdminAccess">Checks if user should be editor or admin. False (editor) by default.</param>
        /// <typeparam name="T">Type of service response.</typeparam>
        /// <returns>Returns 'true' if part guide is editable, service response and part guide itself.</returns>
        private async Task<Tuple<bool, ServiceResponse<T>, PartGuide>> PartGuideIsEditable<T>(int partGuideId,
            bool requiresAdminAccess = false)
        {
            var serviceResponse = new ServiceResponse<T>();

            var partGuide = await _guidesRepository.GetPartGuide(partGuideId);

            if (partGuide == null)
            {
                serviceResponse.StatusCode = 404;
                serviceResponse.Message = "Part guide with this id was not found.";
                serviceResponse.MessageRu = "Гайд детали с таким id не найден.";
                return new Tuple<bool, ServiceResponse<T>, PartGuide>(false, serviceResponse, null);
            }

            var (isEditable, accessResponse, guide) = await GuideIsEditable<T>(partGuide.GuideId, requiresAdminAccess);

            return new Tuple<bool, ServiceResponse<T>, PartGuide>(isEditable, accessResponse, partGuide);
        }
    }
}