using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using GuidesFusion360Server.Data;
using GuidesFusion360Server.Dtos;
using GuidesFusion360Server.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace GuidesFusion360Server.Services
{
    /// <inheritdoc />
    public class GuidesService : IGuidesService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly IAuthRepository _authRepo;
        private readonly IFileManager _fileManager;
        private readonly IHttpClientFactory _clientFactory;

        public GuidesService(IMapper mapper, DataContext context, IAuthRepository authRepo, IFileManager fileManager,
            IHttpClientFactory clientFactory)
        {
            _mapper = mapper;
            _context = context;
            _authRepo = authRepo;
            _fileManager = fileManager;
            _clientFactory = clientFactory;
        }

        /// <inheritdoc />
        public async Task<ServiceResponseModel<List<GetAllGuidesDto>>> GetAllGuides()
        {
            var serviceResponse = new ServiceResponseModel<List<GetAllGuidesDto>>();

            var guides = await _context.Guides.Where(x => x.Hidden == "false").ToListAsync();

            serviceResponse.Data = guides.Select(c => _mapper.Map<GetAllGuidesDto>(c)).ToList();
            return serviceResponse;
        }

        /// <inheritdoc />
        public async Task<ServiceResponseModel<List<GetAllGuidesDto>>> GetAllHiddenGuides(int userId)
        {
            var serviceResponse = new ServiceResponseModel<List<GetAllGuidesDto>>();

            var hasAccess = await _authRepo.UserIsEditor(userId);

            if (!hasAccess)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "User access should be editor or admin.";
                return serviceResponse;
            }

            var guides = await _context.Guides.Where(x => x.Hidden == "true").ToListAsync();

            serviceResponse.Data = guides.Select(c => _mapper.Map<GetAllGuidesDto>(c)).ToList();
            return serviceResponse;
        }

        /// <inheritdoc />
        public async Task<Tuple<ServiceResponseModel<List<GetAllPartGuidesDto>>, int>> GetAllPublicPartGuides(
            int guideId)
        {
            var serviceResponse = new ServiceResponseModel<List<GetAllPartGuidesDto>>();

            var (isAvailable, accessResponse, statusCode) = await GuideIsPublic<List<GetAllPartGuidesDto>>(guideId);

            if (!isAvailable)
            {
                return new Tuple<ServiceResponseModel<List<GetAllPartGuidesDto>>, int>(accessResponse, statusCode);
            }

            var guides = await _context.PartGuides.Where(x => x.GuideId == guideId).ToListAsync();

            serviceResponse.Data = guides.Select(c => _mapper.Map<GetAllPartGuidesDto>(c)).ToList();
            return new Tuple<ServiceResponseModel<List<GetAllPartGuidesDto>>, int>(serviceResponse, 200);
        }

        /// <inheritdoc />
        public async Task<ServiceResponseModel<List<GetAllPartGuidesDto>>> GetAllPrivatePartGuides(int guideId,
            int userId)
        {
            var serviceResponse = new ServiceResponseModel<List<GetAllPartGuidesDto>>();

            var hasAccess = await _authRepo.UserIsEditor(userId);

            if (!hasAccess)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "User access should be editor or admin.";
                return serviceResponse;
            }

            var guides = await _context.PartGuides.Where(x => x.GuideId == guideId).ToListAsync();

            serviceResponse.Data = guides.Select(c => _mapper.Map<GetAllPartGuidesDto>(c)).ToList();
            return serviceResponse;
        }

        /// <inheritdoc />
        public async Task<Tuple<ServiceResponseModel<FileContentResult>, int>> GetPublicGuideFile(int guideId,
            string filename)
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
                return new Tuple<ServiceResponseModel<FileContentResult>, int>(accessResponse, statusCode);
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
        public async Task<ServiceResponseModel<FileContentResult>> GetPrivateGuideFile(int guideId, string filename,
            int userId)
        {
            var serviceResponse = new ServiceResponseModel<FileContentResult>();

            var hasAccess = await _authRepo.UserIsEditor(userId);

            if (!hasAccess)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "User access should be editor or admin.";
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
        public async Task<Tuple<ServiceResponseModel<int>, int>> CreateNewGuide(int userId, AddNewGuideDto newGuide)
        {
            var serviceResponse = new ServiceResponseModel<int>();

            var hasAccess = await _authRepo.UserIsEditor(userId);
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

            await _context.Guides.AddAsync(guide);
            await _context.SaveChangesAsync();

            await _fileManager.SaveFile(guide.Id, "preview.png", newGuide.Image);

            serviceResponse.Data = guide.Id;
            serviceResponse.Message = "Guide is successfully added.";
            return new Tuple<ServiceResponseModel<int>, int>(serviceResponse, 200);
        }

        /// <inheritdoc />
        public async Task<Tuple<ServiceResponseModel<int>, int>> CreateNewPartGuide(int userId,
            AddNewPartGuideDto newPartGuide)
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

            await _context.PartGuides.AddAsync(partGuide);
            await _context.SaveChangesAsync();

            serviceResponse.Data = partGuide.Id;
            serviceResponse.Message = "Part guide is successfully added.";
            return new Tuple<ServiceResponseModel<int>, int>(serviceResponse, 200);
        }

        /// <inheritdoc />
        public async Task<Tuple<ServiceResponseModel<int>, int>> UploadModel(int userId, AddNewGuideModelDto newModel)
        {
            var serviceResponse = new ServiceResponseModel<int>();

            var (isEditable, accessResponse, statusCode, guide) =
                await GuideIsEditable<int>(userId, newModel.GuideId);

            if (!isEditable)
            {
                return new Tuple<ServiceResponseModel<int>, int>(accessResponse, statusCode);
            }

            using var formData = new MultipartFormDataContent();

            // 'File' field
            var bodyFile = new StreamContent(newModel.File.OpenReadStream());
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

            // MPU Cloud Exchanger
            using var client = _clientFactory.CreateClient("converter");
            var response = await client.PostAsync("/model", formData);
            var data = JObject.Parse(await response.Content.ReadAsStringAsync());
            var glbModel = Convert.FromBase64String(data["output"].ToString());

            await _fileManager.SaveFile(newModel.GuideId, "model.glb", glbModel);

            serviceResponse.Data = newModel.GuideId;
            return new Tuple<ServiceResponseModel<int>, int>(serviceResponse, 200);
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
            _context.Guides.Update(guide);
            await _context.SaveChangesAsync();

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

            _context.PartGuides.Update(partGuide);
            await _context.SaveChangesAsync();

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

            var partGuide2 = await _context.PartGuides.FirstOrDefaultAsync(x => x.Id == partGuideId2);

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

            _context.PartGuides.Update(partGuide1);
            _context.PartGuides.Update(partGuide2);
            await _context.SaveChangesAsync();

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

            var partGuides = await _context.PartGuides.Where(x => x.GuideId == guideId).ToListAsync();
            foreach (var partGuide in partGuides)
            {
                _context.PartGuides.Remove(partGuide);
            }

            _context.Guides.Remove(guide);
            await _context.SaveChangesAsync();

            serviceResponse.Message = "Guide is deleted successfully.";
            return new Tuple<ServiceResponseModel<int>, int>(serviceResponse, 200);
        }

        /// <inheritdoc />
        public async Task<Tuple<ServiceResponseModel<int>, int>> RemovePartGuide(int userId, int partGuideId)
        {
            var serviceResponse = new ServiceResponseModel<int>();

            var (isEditable, accessResponse, statusCode, partGuide) =
                await PartGuideIsEditable<int>(userId, partGuideId, true);

            if (!isEditable)
            {
                return new Tuple<ServiceResponseModel<int>, int>(accessResponse, statusCode);
            }

            _context.PartGuides.Remove(partGuide);
            await _context.SaveChangesAsync();

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

            var guide = await _context.Guides.FirstOrDefaultAsync(x => x.Id == guideId);

            if (guide == null)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "Guide with this id was not found.";
                return new Tuple<bool, ServiceResponseModel<T>, int>(false, serviceResponse, 404);
            }

            if (guide.Hidden == "true")
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "Guide with this id is not public. You should provide token to access it.";
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
            int guideId,
            bool requiresAdminAccess = false)
        {
            var serviceResponse = new ServiceResponseModel<T>();

            var hasAccess = requiresAdminAccess
                ? await _authRepo.UserIsAdmin(userId)
                : await _authRepo.UserIsEditor(userId);

            if (!hasAccess)
            {
                serviceResponse.Success = false;
                serviceResponse.Message =
                    $"User access should be {(requiresAdminAccess ? "admin." : "editor or admin.")}";
                return new Tuple<bool, ServiceResponseModel<T>, int, GuideModel>(false, serviceResponse, 401, null);
            }

            var guide = await _context.Guides.FirstOrDefaultAsync(x => x.Id == guideId);

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

            var partGuide = await _context.PartGuides.FirstOrDefaultAsync(x => x.Id == partGuideId);

            if (partGuide == null)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "Part guide with this id was not found.";
                return new Tuple<bool, ServiceResponseModel<T>, int, PartGuideModel>(false, serviceResponse, 404, null);
            }

            var (isEditable, accessResponse, statusCode, guide) =
                await GuideIsEditable<T>(userId, partGuideId, requiresAdminAccess);

            return new Tuple<bool, ServiceResponseModel<T>, int, PartGuideModel>(isEditable, accessResponse, statusCode,
                partGuide);
        }
    }
}