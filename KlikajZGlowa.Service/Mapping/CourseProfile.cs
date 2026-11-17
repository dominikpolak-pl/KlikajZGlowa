using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using KlikajZGlowa.Service.DTO.Course;
using KlikajZGlowa.Model;

namespace KlikajZGlowa.Service.Mapping
{
    public class CourseProfile : Profile
    {
        public CourseProfile()
        {
            CreateMap<CreateCourseDto, Course>()
                .ForMember(dest => dest.CourseId, opt => opt.Ignore())
                .ForMember(dest => dest.Lessons, opt => opt.Ignore())
                .ForMember(dest => dest.Authors, opt => opt.Ignore())
                .ForMember(dest => dest.EnrolledUsers, opt => opt.Ignore());
            CreateMap<Course, CourseDto>()
                .ForMember(dest => dest.IsEnrolled, opt => opt.Ignore());
            CreateMap<UpdateCourseDto, Course>()
                .ForMember(dest => dest.CourseId, opt => opt.Ignore())
                .ForMember(dest => dest.Lessons, opt => opt.Ignore())
                .ForMember(dest => dest.Authors, opt => opt.Ignore())
                .ForMember(dest => dest.EnrolledUsers, opt => opt.Ignore());
            CreateMap<Course, DetailsCourseDto>()
                .ForMember(dest => dest.IsEnrolled, opt => opt.Ignore())
                .ForMember(dest => dest.IsAuthor, opt => opt.Ignore());
        }
    }
}
