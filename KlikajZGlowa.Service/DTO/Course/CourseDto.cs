using KlikajZGlowa.Model;
using KlikajZGlowa.Service.DTO.Author;
using KlikajZGlowa.Service.DTO.Lesson;
using System;
using System.Collections.Generic;
using System.Text;

namespace KlikajZGlowa.Service.DTO.Course
{
    public class CourseDto
    {
        public int CourseId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsEnrolled { get; set; }
    }
}
