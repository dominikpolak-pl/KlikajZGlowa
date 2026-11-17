using AutoMapper;
using KlikajZGlowa.Service.DTO.Author;
using KlikajZGlowa.Model;

namespace KlikajZGlowa.Service.Mapping
{
    public class AuthorProfile : Profile
    {
        public AuthorProfile()
        {
            CreateMap<CreateAuthorDto, Author>()
                .ForMember(dest => dest.AuthorId, opt => opt.Ignore())
                .ForMember(dest => dest.Courses, opt => opt.Ignore())
                .ForMember(dest => dest.ApplicationUser, opt => opt.Ignore());
            CreateMap<Author, AuthorDto>();
            CreateMap<UpdateAuthorDto, Author>()
                .ForMember(dest => dest.AuthorId, opt => opt.Ignore())
                .ForMember(dest => dest.Courses, opt => opt.Ignore())
                .ForMember(dest => dest.ApplicationUserId, opt => opt.Ignore())
                .ForMember(dest => dest.ApplicationUser, opt => opt.Ignore());
            CreateMap<Author, DetailsAuthorDto>();
            CreateMap<ApplicationUser, Author>()
                .ForMember(dest => dest.ApplicationUserId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.AuthorId, opt => opt.Ignore())
                .ForMember(dest => dest.ApplicationUser, opt => opt.Ignore())
                .ForMember(dest => dest.Courses, opt => opt.Ignore());
        }
    }
}
