using System;
using System.Collections.Generic;
using System.Text;

namespace KlikajZGlowa.Service.DTO.Lesson
{
    public class DetailsLessonDto
    {
        public int LessonId { get; set; }
        public string Title { get; set; }
        public string ContentArea { get; set; }
        public int CourseId { get; set; }
    }
}
