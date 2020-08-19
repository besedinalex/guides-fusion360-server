using System.Collections.Generic;
using System.Threading.Tasks;
using GuidesFusion360Server.Dtos;
using GuidesFusion360Server.Models;

namespace GuidesFusion360Server.Services
{
    public interface IGuidesService
    {
        Task<ServiceResponse<List<GetAllGuidesDto>>> GetAllGuides();
        
        Task<ServiceResponse<List<GetAllPartGuidesDto>>> GetAllPartGuideData(int guideId);

        Task<ServiceResponse<int>> CreateNewGuide(string name, string description, int ownerId);
    }
}
