using AutoMapper;
using GuidesFusion360Server.Dtos.Guides;
using GuidesFusion360Server.Dtos.ModelAnnotations;
using GuidesFusion360Server.Dtos.Users;
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
            CreateMap<Guide, GetGuideDto>();
            CreateMap<PartGuide, GetPartGuideDto>();
            CreateMap<User, GetGuideOwnerDto>();
            CreateMap<AddGuideDto, Guide>();
            CreateMap<AddPartGuideDto, PartGuide>();
            CreateMap<User, GetUserDto>();
            CreateMap<Guide, GetUserGuideDto>();
            CreateMap<ModelAnnotation, GetModelAnnotationDto>();
            CreateMap<AddModelAnnotationDto, ModelAnnotation>();
        }
    }
}