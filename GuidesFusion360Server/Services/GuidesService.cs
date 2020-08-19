using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GuidesFusion360Server.Data;
using GuidesFusion360Server.Dtos;
using GuidesFusion360Server.Models;
using Microsoft.EntityFrameworkCore;

namespace GuidesFusion360Server.Services
{
    public class GuidesService : IGuidesService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;

        public GuidesService(IMapper mapper, DataContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<ServiceResponse<List<GetAllGuidesDto>>> GetAllGuides()
        {
            var serviceResponse = new ServiceResponse<List<GetAllGuidesDto>>();
            // TODO: Hidden should be false
            var guides = await _context.Guides.Where(x => x.Hidden == "true").ToListAsync();
            serviceResponse.Data = guides.Select(c => _mapper.Map<GetAllGuidesDto>(c)).ToList();
            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetAllPartGuidesDto>>> GetAllPartGuideData(int guideId)
        {
            var serviceResponse = new ServiceResponse<List<GetAllPartGuidesDto>>();
            var guides = await _context.PartGuides.Where(x => x.GuideId == guideId).ToListAsync();
            serviceResponse.Data = guides.Select(c => _mapper.Map<GetAllPartGuidesDto>(c)).ToList();
            return serviceResponse;
        }

        public async Task<ServiceResponse<int>> CreateNewGuide(int ownerId, AddNewGuideDto newGuide)
        {
            var serviceResponse = new ServiceResponse<int>();
            
            var guide = _mapper.Map<Guide>(newGuide);
            guide.OwnerId = ownerId;
            guide.Hidden = "true";
            
            await _context.Guides.AddAsync(guide);
            await _context.SaveChangesAsync();

            serviceResponse.Data = guide.Id;
            serviceResponse.Message = "Guide is added.";
            return serviceResponse;
        }
    }
}
