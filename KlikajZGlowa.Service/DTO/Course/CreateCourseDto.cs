using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace KlikajZGlowa.Service.DTO.Course
{
    public class CreateCourseDto
    {
        [Required]
        public string Title { get; set; }

        public string Description { get; set; }
    }
}
