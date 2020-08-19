using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GuidesFusion360Server.Data;
using GuidesFusion360Server.Dtos;
using GuidesFusion360Server.Models;

namespace GuidesFusion360Server.Services
{
    public class GuidesService : IGuidesService
    {
        private readonly IMapper _mapper;

        public GuidesService(IMapper mapper)
        {
            _mapper = mapper;
        }

        public async Task<ServiceResponse<List<GetAllGuidesDto>>> GetAllGuides()
        {
            var serviceResponse = new ServiceResponse<List<GetAllGuidesDto>>();
            var guides = GuidesDatabase.SelectGuides("false");
            serviceResponse.Data = guides.Select(c => _mapper.Map<GetAllGuidesDto>(c)).ToList();
            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetAllPartGuidesDto>>> GetAllPartGuideData(int guideId)
        {
            var serviceResponse = new ServiceResponse<List<GetAllPartGuidesDto>>();
            var guides = GuidesDatabase.SelectPartGuides(guideId);
            serviceResponse.Data = guides.Select(c => _mapper.Map<GetAllPartGuidesDto>(c)).ToList();
            return serviceResponse;
        }

        public async Task<ServiceResponse<int>> CreateNewGuide(string name, string description, int ownerId)
        {
            var serviceResponse = new ServiceResponse<int>();
            var lastId = GuidesDatabase.InsertGuide(name, description, ownerId);
            serviceResponse.Data = lastId;
            serviceResponse.Message = "Guide is added.";
            return serviceResponse;
        }
    }
}
