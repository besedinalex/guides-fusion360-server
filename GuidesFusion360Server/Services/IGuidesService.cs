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

        Task<Tuple<ServiceResponse<FileContentResult>, int>> GetPublicGuidePreview(int guideId);
        
        Task<ServiceResponse<FileContentResult>> GetPrivateGuidePreview(int guideId, int userId);
        
        Task<ServiceResponse<List<GetAllPartGuidesDto>>> GetAllPartGuideData(int guideId);

        Task<ServiceResponse<int>> CreateNewGuide(int ownerId, AddNewGuideDto newGuide);
    }
}
