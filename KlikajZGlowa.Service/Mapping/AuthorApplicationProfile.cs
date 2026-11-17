using AutoMapper;
using KlikajZGlowa.Model;
using KlikajZGlowa.Service.DTO.AuthorApplication;
using System;
using System.Collections.Generic;
using System.Text;

namespace KlikajZGlowa.Service.Mapping
{
    public class AuthorApplicationProfile : Profile
    {
        public AuthorApplicationProfile()
        {
            CreateMap<AuthorApplication, AuthorApplicationDto>()
                .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.ApplicationUser != null ? src.ApplicationUser.Email : string.Empty))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.ApplicationUser != null ? src.ApplicationUser.FirstName + " " + src.ApplicationUser.LastName : string.Empty));

            CreateMap<CreateAuthorApplicationDto, AuthorApplication>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => ApplicationStatus.Pending))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ApplicationUserId, opt => opt.Ignore())
                .ForMember(dest => dest.ApplicationUser, opt => opt.Ignore())
                .ForMember(dest => dest.RejectionReason, opt => opt.Ignore());
        }
    }
}
