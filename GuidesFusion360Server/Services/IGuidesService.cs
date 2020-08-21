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

        Task<Tuple<ServiceResponse<FileContentResult>, int>> GetPublicGuidePreview(int guideId);
        
        Task<ServiceResponse<FileContentResult>> GetPrivateGuidePreview(int guideId, int userId);
        
        Task<Tuple<ServiceResponse<List<GetAllPartGuidesDto>>, int>> GetAllPublicPartGuides(int guideId);

        Task<ServiceResponse<List<GetAllPartGuidesDto>>> GetAllPrivatePartGuides(int guideId, int userId);

        Task<Tuple<ServiceResponse<int>, int>> CreateNewGuide(int ownerId, AddNewGuideDto newGuide);

        Task<Tuple<ServiceResponse<int>, int>> CreateNewPartGuide(int ownerId, AddNewPartGuideDto newGuide);
    }
}
