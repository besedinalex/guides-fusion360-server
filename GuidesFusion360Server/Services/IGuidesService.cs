using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GuidesFusion360Server.Dtos;
using GuidesFusion360Server.Models;
using Microsoft.AspNetCore.Mvc;

namespace GuidesFusion360Server.Services
{
    public interface IGuidesService
    {
        Task<ServiceResponse<List<GetAllGuidesDto>>> GetAllGuides();

        Task<ServiceResponse<List<GetAllGuidesDto>>> GetAllHiddenGuides(int userId);

        Task<Tuple<ServiceResponse<List<GetAllPartGuidesDto>>, int>> GetAllPublicPartGuides(int guideId);

        Task<ServiceResponse<List<GetAllPartGuidesDto>>> GetAllPrivatePartGuides(int guideId, int userId);
        
        Task<Tuple<ServiceResponse<FileContentResult>, int>> GetPublicGuideFile(int guideId, string filename);
        
        Task<ServiceResponse<FileContentResult>> GetPrivateGuideFile(int guideId, string filename, int userId);

        Task<Tuple<ServiceResponse<int>, int>> CreateNewGuide(int ownerId, AddNewGuideDto newGuide);

        Task<Tuple<ServiceResponse<int>, int>> CreateNewPartGuide(int ownerId, AddNewPartGuideDto newPartGuide);

        Task<Tuple<ServiceResponse<int>, int>> UploadModel(int userId, AddNewGuideModelDto newModel);

        Task<Tuple<ServiceResponse<int>, int>> ChangeGuideVisibility(int userId, int guideId, string hidden);

        Task<Tuple<ServiceResponse<int>, int>> UpdatePartGuide(int userId, int partGuideId,
            UpdatePartGuideDto updatedGuide);

        Task<Tuple<ServiceResponse<int>, int>> SwitchPartGuides(int userId, int partGuideId1, int partGuideId2);

        Task<Tuple<ServiceResponse<int>, int>> RemoveGuide(int userId, int guideId);
        
        Task<Tuple<ServiceResponse<int>, int>> RemovePartGuide(int userId, int partGuideId);
    }
}
