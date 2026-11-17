using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using KlikajZGlowa.Service.DTO.Lesson;
using KlikajZGlowa.Model;
namespace KlikajZGlowa.Service.Mapping
{
    public class LessonProfile : Profile
    {
        public LessonProfile()
        {
            CreateMap<CreateLessonDto, Lesson>()
                .ForMember(dest => dest.LessonId, opt => opt.Ignore())
                .ForMember(dest => dest.Course, opt => opt.Ignore());
            CreateMap<Lesson, LessonDto>();
            CreateMap<Lesson, DetailsLessonDto>();
            CreateMap<UpdateLessonDto, Lesson>()
                .ForMember(dest => dest.LessonId, opt => opt.Ignore())
                .ForMember(dest => dest.CourseId, opt => opt.Ignore())
                .ForMember(dest => dest.Course, opt => opt.Ignore());
        }
    }
}
