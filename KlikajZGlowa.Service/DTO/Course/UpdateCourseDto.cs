using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace KlikajZGlowa.Service.DTO.Course
{
    public class UpdateCourseDto
    {
        public int CourseId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
