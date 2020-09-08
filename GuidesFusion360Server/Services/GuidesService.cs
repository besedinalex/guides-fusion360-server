using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using GuidesFusion360Server.Data;
using GuidesFusion360Server.Data.Repositories;
using GuidesFusion360Server.Dtos;
using GuidesFusion360Server.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace GuidesFusion360Server.Services
{
    /// <inheritdoc />
    public class GuidesService : IGuidesService
    {
        private readonly IMapper _mapper;
        private readonly IFileManager _fileManager;
        private readonly IGuidesRepository _guidesRepository;
        private readonly IUsersRepository _usersRepository;
        private readonly IModelAnnotationsRepository _modelAnnotationsRepository;
        private readonly IHttpClientFactory _clientFactory;

        public GuidesService(IMapper mapper, IFileManager fileManager, IGuidesRepository guidesRepository,
            IUsersRepository usersRepository, IModelAnnotationsRepository modelAnnotationsRepository,
            IHttpClientFactory clientFactory)
        {
            _mapper = mapper;
            _fileManager = fileManager;
            _guidesRepository = guidesRepository;
            _usersRepository = usersRepository;
            _modelAnnotationsRepository = modelAnnotationsRepository;
            _clientFactory = clientFactory;
        }

        /// <inheritdoc />
        public async Task<ServiceResponseModel<List<GetGuidesDto>>> GetAllPublicGuides()
        {
            var serviceResponse = new ServiceResponseModel<List<GetGuidesDto>>();

            var guides = await _guidesRepository.GetAllPublicGuides();

            serviceResponse.Data = guides.Select(c => _mapper.Map<GetGuidesDto>(c)).ToList();
            return serviceResponse;
        }

        /// <inheritdoc />
        public async Task<ServiceResponseModel<List<GetGuidesDto>>> GetAllHiddenGuides(int userId)
        {
            var serviceResponse = new ServiceResponseModel<List<GetGuidesDto>>();

            var hasAccess = await _usersRepository.UserIsEditor(userId);

            if (!hasAccess)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "User access should be editor or admin.";
                return serviceResponse;
            }

            var guides = await _guidesRepository.GetAllHiddenGuides();

            serviceResponse.Data = guides.Select(c => _mapper.Map<GetGuidesDto>(c)).ToList();
            return serviceResponse;
        }

        /// <inheritdoc />
        public async Task<Tuple<ServiceResponseModel<List<GetPartGuidesDto>>, int>> GetPartGuides(int guideId,
            int userId)
        {
            var serviceResponse = new ServiceResponseModel<List<GetPartGuidesDto>>();

            var (isAvailable, accessResponse, statusCode) = await GuideIsPublic<List<GetPartGuidesDto>>(guideId);

            if (!isAvailable)
            {
                if (userId == -1)
                {
                    return new Tuple<ServiceResponseModel<List<GetPartGuidesDto>>, int>(accessResponse, statusCode);
                }

                var hasAccess = await _usersRepository.UserIsEditor(userId);

                if (!hasAccess)
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "User access should be editor or admin.";
                    return new Tuple<ServiceResponseModel<List<GetPartGuidesDto>>, int>(serviceResponse, 401);
                }
            }

            var guides = await _guidesRepository.GetPartGuides(guideId);

            serviceResponse.Data = guides.Select(c => _mapper.Map<GetPartGuidesDto>(c)).ToList();
            return new Tuple<ServiceResponseModel<List<GetPartGuidesDto>>, int>(serviceResponse, 200);
        }

        /// <inheritdoc />
        public async Task<Tuple<ServiceResponseModel<FileContentResult>, int>> GetGuideFile(int guideId,
            string filename, int userId)
        {
            var serviceResponse = new ServiceResponseModel<FileContentResult>();

            if (!_fileManager.FileExists(guideId, filename))
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "File is not found.";
                return new Tuple<ServiceResponseModel<FileContentResult>, int>(serviceResponse, 404);
            }

            var (isAvailable, accessResponse, statusCode) = await GuideIsPublic<FileContentResult>(guideId);

            if (!isAvailable)
            {
                if (userId == -1)
                {
                    return new Tuple<ServiceResponseModel<FileContentResult>, int>(accessResponse, statusCode);
                }

                var hasAccess = await _usersRepository.UserIsEditor(userId);

                if (!hasAccess)
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "User access should be editor or admin.";
                    return new Tuple<ServiceResponseModel<FileContentResult>, int>(serviceResponse, 401);
                }
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
            return new Tuple<ServiceResponseModel<FileContentResult>, int>(serviceResponse, 200);
        }

        /// <inheritdoc />
        public async Task<Tuple<ServiceResponseModel<GetGuideOwnerDto>, int>> GetGuideOwner(int guideId, int userId)
        {
            var serviceResponse = new ServiceResponseModel<GetGuideOwnerDto>();

            var hasAccess = await _usersRepository.UserIsEditor(userId);
            if (!hasAccess)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "User access should be editor or admin.";
                return new Tuple<ServiceResponseModel<GetGuideOwnerDto>, int>(serviceResponse, 401);
            }

            var guide = await _guidesRepository.GetGuide(guideId);
            if (guide == null)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "Guide is not found.";
                return new Tuple<ServiceResponseModel<GetGuideOwnerDto>, int>(serviceResponse, 404);
            }
            
            var owner = await _usersRepository.GetUser(guide.OwnerId);

            serviceResponse.Data = _mapper.Map<GetGuideOwnerDto>(owner);
            return new Tuple<ServiceResponseModel<GetGuideOwnerDto>, int>(serviceResponse, 200);
        }

        /// <inheritdoc />
        public async Task<Tuple<ServiceResponseModel<int>, int>> CreateGuide(int userId, AddGuideDto newGuide)
        {
            var serviceResponse = new ServiceResponseModel<int>();

            var hasAccess = await _usersRepository.UserIsEditor(userId);
            if (!hasAccess)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "User access should be editor or admin.";
                return new Tuple<ServiceResponseModel<int>, int>(serviceResponse, 401);
            }

            if (newGuide.Image.ContentType != "image/png")
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "File must be PNG image.";
                return new Tuple<ServiceResponseModel<int>, int>(serviceResponse, 400);
            }

            var guide = _mapper.Map<GuideModel>(newGuide);
            guide.OwnerId = userId;
            guide.Hidden = "true";

            var guideId = await _guidesRepository.CreateGuide(guide);
            await _fileManager.SaveFile(guideId, "preview.png", newGuide.Image);

            serviceResponse.Data = guideId;
            serviceResponse.Message = "Guide is successfully added.";
            return new Tuple<ServiceResponseModel<int>, int>(serviceResponse, 200);
        }

        /// <inheritdoc />
        public async Task<Tuple<ServiceResponseModel<int>, int>> CreatePartGuide(int userId,
            AddPartGuideDto newPartGuide)
        {
            var serviceResponse = new ServiceResponseModel<int>();

            var (isEditable, accessResponse, statusCode, guide) =
                await GuideIsEditable<int>(userId, newPartGuide.GuideId);

            if (!isEditable)
            {
                return new Tuple<ServiceResponseModel<int>, int>(accessResponse, statusCode);
            }

            var content = newPartGuide.Content;
            var file = newPartGuide.File;

            if (file == null && content == null || file != null && content != null)
            {
                serviceResponse.Success = false;
                serviceResponse.Message =
                    "You should provide PDF or ZIP in file field or YouTube link in content field.";
                return new Tuple<ServiceResponseModel<int>, int>(serviceResponse, 400);
            }

            var partGuide = _mapper.Map<PartGuideModel>(newPartGuide);

            if (content != null && !Uri.IsWellFormedUriString(content, UriKind.Absolute))
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "You should provide valid URL in link field.";
                return new Tuple<ServiceResponseModel<int>, int>(serviceResponse, 400);
            }

            if (file != null)
            {
                var isPdf = file.ContentType == "application/pdf";
                var isZip = file.ContentType == "application/zip" || file.ContentType == "application/x-zip-compressed";
                if (!isPdf && !isZip)
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "You should provide PDF or ZIP file.";
                    return new Tuple<ServiceResponseModel<int>, int>(serviceResponse, 400);
                }

                if (_fileManager.FileExists(partGuide.GuideId, file.FileName))
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "File with this name already exists in this guide.";
                    return new Tuple<ServiceResponseModel<int>, int>(serviceResponse, 400);
                }

                partGuide.Content = file.FileName;
                await _fileManager.SaveFile(partGuide.GuideId, file.FileName, file);
            }

            var partGuideId = await _guidesRepository.CreatePartGuide(partGuide);

            serviceResponse.Data = partGuideId;
            serviceResponse.Message = "Part guide is successfully added.";
            return new Tuple<ServiceResponseModel<int>, int>(serviceResponse, 200);
        }

        /// <inheritdoc />
        public async Task<Tuple<ServiceResponseModel<int>, int>> UploadModel(int userId, AddGuideModelDto model)
        {
            var serviceResponse = new ServiceResponseModel<int>();

            var (isEditable, accessResponse, statusCode, guide) =
                await GuideIsEditable<int>(userId, model.GuideId);

            if (!isEditable)
            {
                return new Tuple<ServiceResponseModel<int>, int>(accessResponse, statusCode);
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

            try
            {
                // MPU Cloud Exchanger
                using var client = _clientFactory.CreateClient("converter");
                var response = await client.PostAsync("/model", formData);
                response.EnsureSuccessStatusCode();
                var data = JObject.Parse(await response.Content.ReadAsStringAsync());
                var glbModel = Convert.FromBase64String(data["output"].ToString());

                await _fileManager.SaveFile(model.GuideId, "model.glb", glbModel);

                serviceResponse.Data = model.GuideId;
                serviceResponse.Message = "Model was converted to and saved as glb successfully.";
                return new Tuple<ServiceResponseModel<int>, int>(serviceResponse, 200);
            }
            catch
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "Unable to convert the model.";
                return new Tuple<ServiceResponseModel<int>, int>(serviceResponse, 500);
            }
        }

        /// <inheritdoc />
        public async Task<Tuple<ServiceResponseModel<int>, int>> ChangeGuideVisibility(int userId, int guideId,
            string hidden)
        {
            var serviceResponse = new ServiceResponseModel<int>();

            var (isEditable, accessResponse, statusCode, guide) =
                await GuideIsEditable<int>(userId, guideId, true);

            if (!isEditable)
            {
                return new Tuple<ServiceResponseModel<int>, int>(accessResponse, statusCode);
            }

            hidden = hidden.ToLower();

            if (hidden != "true" && hidden != "false")
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "Hidden field should be true or false.";
                return new Tuple<ServiceResponseModel<int>, int>(serviceResponse, 400);
            }

            guide.Hidden = hidden;

            await _guidesRepository.UpdateGuide(guide);

            serviceResponse.Data = guideId;
            serviceResponse.Message = hidden == "true" ? "Guide is now hidden." : "Guide is now public.";
            return new Tuple<ServiceResponseModel<int>, int>(serviceResponse, 200);
        }

        /// <inheritdoc />
        public async Task<Tuple<ServiceResponseModel<int>, int>> UpdatePartGuide(int userId, int partGuideId,
            UpdatePartGuideDto updatedGuide)
        {
            var serviceResponse = new ServiceResponseModel<int>();

            var (isEditable, accessResponse, statusCode, partGuide) =
                await PartGuideIsEditable<int>(userId, partGuideId);

            if (!isEditable)
            {
                return new Tuple<ServiceResponseModel<int>, int>(accessResponse, statusCode);
            }

            var name = updatedGuide.Name;
            var content = updatedGuide.Content;
            var file = updatedGuide.File;

            if (file != null && content != null)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "You cannot provide file and content at the same time.";
                return new Tuple<ServiceResponseModel<int>, int>(serviceResponse, 400);
            }

            if (content != null)
            {
                if (!Uri.IsWellFormedUriString(content, UriKind.Absolute))
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "You should provide valid URL in content field.";
                    return new Tuple<ServiceResponseModel<int>, int>(serviceResponse, 400);
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
                    serviceResponse.Success = false;
                    serviceResponse.Message = "You should provide PDF or ZIP file or no files at all.";
                    return new Tuple<ServiceResponseModel<int>, int>(serviceResponse, 400);
                }

                if (_fileManager.FileExists(partGuide.GuideId, file.FileName))
                {
                    if (file.FileName != partGuide.Content)
                    {
                        serviceResponse.Success = false;
                        serviceResponse.Message = "File with this name already exists in this guide.";
                        return new Tuple<ServiceResponseModel<int>, int>(serviceResponse, 400);
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
            return new Tuple<ServiceResponseModel<int>, int>(serviceResponse, 200);
        }

        /// <inheritdoc />
        public async Task<Tuple<ServiceResponseModel<int>, int>> SwitchPartGuides(int userId, int partGuideId1,
            int partGuideId2)
        {
            var serviceResponse = new ServiceResponseModel<int>();

            var (isEditable, accessResponse, statusCode, partGuide1) =
                await PartGuideIsEditable<int>(userId, partGuideId1);

            if (!isEditable)
            {
                return new Tuple<ServiceResponseModel<int>, int>(accessResponse, statusCode);
            }

            var partGuide2 = await _guidesRepository.GetPartGuide(partGuideId2);

            if (partGuide2 == null)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "Part guide with this id was not found";
                return new Tuple<ServiceResponseModel<int>, int>(serviceResponse, 404);
            }

            if (partGuide1.GuideId != partGuide2.GuideId)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "Part guides should be from one guide.";
                return new Tuple<ServiceResponseModel<int>, int>(serviceResponse, 400);
            }

            var partGuide1SortKey = partGuide1.SortKey;
            partGuide1.SortKey = partGuide2.SortKey;
            partGuide2.SortKey = partGuide1SortKey;

            await _guidesRepository.UpdatePartGuides(new List<PartGuideModel> {partGuide1, partGuide2});

            serviceResponse.Message = "Part guides are switched successfully.";
            return new Tuple<ServiceResponseModel<int>, int>(serviceResponse, 200);
        }

        /// <inheritdoc />
        public async Task<Tuple<ServiceResponseModel<int>, int>> RemoveGuide(int userId, int guideId)
        {
            var serviceResponse = new ServiceResponseModel<int>();

            var (isEditable, accessResponse, statusCode, guide) =
                await GuideIsEditable<int>(userId, guideId, true);

            if (!isEditable)
            {
                return new Tuple<ServiceResponseModel<int>, int>(accessResponse, statusCode);
            }

            var guideModelAnnotations = await _modelAnnotationsRepository.GetAnnotations(guideId);

            _fileManager.DeleteFolder(guideId);
            await _modelAnnotationsRepository.DeleteAnnotations(guideModelAnnotations);
            await _guidesRepository.RemoveGuide(guide);

            serviceResponse.Message = "Guide is deleted successfully.";
            return new Tuple<ServiceResponseModel<int>, int>(serviceResponse, 200);
        }

        /// <inheritdoc />
        public async Task<Tuple<ServiceResponseModel<int>, int>> RemovePartGuide(int userId, int partGuideId)
        {
            var serviceResponse = new ServiceResponseModel<int>();

            var (isEditable, accessResponse, statusCode, partGuide) =
                await PartGuideIsEditable<int>(userId, partGuideId);

            if (!isEditable)
            {
                return new Tuple<ServiceResponseModel<int>, int>(accessResponse, statusCode);
            }

            if (_fileManager.FileExists(partGuide.GuideId, partGuide.Content))
                _fileManager.DeleteFile(partGuide.GuideId, partGuide.Content);
            await _guidesRepository.RemovePartGuide(partGuide);

            serviceResponse.Message = "Part guide is deleted successfully.";
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
        /// <param name="requiresAdminAccess">Checks if user should be editor or admin. False (editor) by default.</param>
        /// <typeparam name="T">Type of service response.</typeparam>
        /// <returns>Returns service response, http code of response and guide itself.</returns>
        private async Task<Tuple<bool, ServiceResponseModel<T>, int, GuideModel>> GuideIsEditable<T>(int userId,
            int guideId, bool requiresAdminAccess = false)
        {
            var serviceResponse = new ServiceResponseModel<T>();

            var hasAccess = requiresAdminAccess
                ? await _usersRepository.UserIsAdmin(userId)
                : await _usersRepository.UserIsEditor(userId);

            if (!hasAccess)
            {
                serviceResponse.Success = false;
                serviceResponse.Message =
                    $"User access should be {(requiresAdminAccess ? "admin." : "editor or admin.")}";
                return new Tuple<bool, ServiceResponseModel<T>, int, GuideModel>(false, serviceResponse, 401, null);
            }

            var guide = await _guidesRepository.GetGuide(guideId);

            if (guide == null)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "Guide with this id was not found.";
                return new Tuple<bool, ServiceResponseModel<T>, int, GuideModel>(false, serviceResponse, 404, null);
            }

            if (!requiresAdminAccess && guide.Hidden == "false")
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "Guide should be hidden in order to edit it.";
                return new Tuple<bool, ServiceResponseModel<T>, int, GuideModel>(false, serviceResponse, 400, null);
            }

            return new Tuple<bool, ServiceResponseModel<T>, int, GuideModel>(true, serviceResponse, 200, guide);
        }

        /// <summary>Checks if part guide is editable with current access level.</summary>
        /// <param name="userId">Id of the user who will edit the guide.</param>
        /// <param name="partGuideId">Id of the part guide.</param>
        /// <param name="requiresAdminAccess">Checks if user should be editor or admin. False (editor) by default.</param>
        /// <typeparam name="T">Type of service response.</typeparam>
        /// <returns>Returns service response, http code of response and part guide itself.</returns>
        private async Task<Tuple<bool, ServiceResponseModel<T>, int, PartGuideModel>> PartGuideIsEditable<T>(int userId,
            int partGuideId, bool requiresAdminAccess = false)
        {
            var serviceResponse = new ServiceResponseModel<T>();

            var partGuide = await _guidesRepository.GetPartGuide(partGuideId);

            if (partGuide == null)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "Part guide with this id was not found.";
                return new Tuple<bool, ServiceResponseModel<T>, int, PartGuideModel>(false, serviceResponse, 404, null);
            }

            var (isEditable, accessResponse, statusCode, guide) =
                await GuideIsEditable<T>(userId, partGuide.GuideId, requiresAdminAccess);

            return new Tuple<bool, ServiceResponseModel<T>, int, PartGuideModel>(isEditable, accessResponse, statusCode,
                partGuide);
        }
    }
}