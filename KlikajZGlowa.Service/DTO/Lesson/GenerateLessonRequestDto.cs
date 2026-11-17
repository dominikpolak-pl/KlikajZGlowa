using System;
using System.Collections.Generic;
using System.Text;

namespace KlikajZGlowa.Service.DTO.Lesson
{
    public class GenerateLessonRequestDto
    {
        public int CourseId { get; set; }
        public string LessonTopic { get; set; } = string.Empty;
        public string? AdditionalInstructions { get; set; }
    }
}
