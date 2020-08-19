using AutoMapper;
using GuidesFusion360Server.Dtos;
using GuidesFusion360Server.Models;

namespace GuidesFusion360Server
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<GuideData, GetAllGuidesDto>();
            CreateMap<PartGuideData, GetAllPartGuidesDto>();
        }
    }
}
