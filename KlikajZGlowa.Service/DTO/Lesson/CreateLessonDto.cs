using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace KlikajZGlowa.Service.DTO.Lesson
{
    public class CreateLessonDto
    {
        [Required]
        public string Title { get; set; } = string.Empty;

        public string ContentArea { get; set; } = string.Empty;

        [Required]
        public int CourseId { get; set; }
    }
}
