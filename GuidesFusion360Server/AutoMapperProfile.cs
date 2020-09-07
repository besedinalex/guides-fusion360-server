using AutoMapper;
using GuidesFusion360Server.Dtos;
using GuidesFusion360Server.Models;

namespace GuidesFusion360Server
{
    /// <summary>
    /// Maps DTOs with models. Makes possible to send or receive data objects that don't match with models entirely.
    /// </summary>
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<GuideModel, GetGuidesDto>();
            CreateMap<PartGuideModel, GetPartGuidesDto>();
            CreateMap<AddGuideDto, GuideModel>();
            CreateMap<AddPartGuideDto, PartGuideModel>();
            CreateMap<UserModel, GetUsersDto>();
            CreateMap<ModelAnnotationModel, GetModelAnnotationsDto>();
            CreateMap<AddModelAnnotationDto, ModelAnnotationModel>();
        }
    }
}