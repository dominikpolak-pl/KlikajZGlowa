using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using KlikajZGlowa.Service.DTO.ApplicationUser;
using KlikajZGlowa.Model;

namespace KlikajZGlowa.Service.Mapping
{
    public class ApplicationUserProfile : Profile
    {
        public ApplicationUserProfile()
        {
            CreateMap<ApplicationUser, ApplicationUserDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
                .ForMember(dest => dest.AuthorId, opt => opt.MapFrom(src => src.Author != null ? src.Author.AuthorId : (int?)null))
                .ForMember(dest => dest.EnrolledUserId, opt => opt.MapFrom(src => src.EnrolledUser != null ? src.EnrolledUser.EnrolledUserId : (int?)null));
            CreateMap<CreateApplicationUserDto, ApplicationUser>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName));
            CreateMap<UpdateApplicationUserDto, ApplicationUser>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber));
            
        }
    }
}
