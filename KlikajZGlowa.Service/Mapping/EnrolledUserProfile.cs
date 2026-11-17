using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using KlikajZGlowa.Service.DTO.EnrolledUser;
using KlikajZGlowa.Model;

namespace KlikajZGlowa.Service.Mapping
{
    public class EnrolledUserProfile : Profile
    {
        public EnrolledUserProfile()
        {
            CreateMap<CreateEnrolledUserDto, EnrolledUser>()
                .ForMember(dest => dest.EnrolledUserId, opt => opt.Ignore())
                .ForMember(dest => dest.Courses, opt => opt.Ignore())
                .ForMember(dest => dest.ApplicationUserId, opt => opt.Ignore())
                .ForMember(dest => dest.ApplicationUser, opt => opt.Ignore());
            CreateMap<EnrolledUser, EnrolledUserDto>();
            CreateMap<UpdateEnrolledUserDto, EnrolledUser>()
                .ForMember(dest => dest.EnrolledUserId, opt => opt.Ignore())
                .ForMember(dest => dest.Courses, opt => opt.Ignore())
                .ForMember(dest => dest.ApplicationUserId, opt => opt.Ignore())
                .ForMember(dest => dest.ApplicationUser, opt => opt.Ignore());
            CreateMap<EnrolledUser, DetailsEnrolledUserDto>();
        }
    }
}
