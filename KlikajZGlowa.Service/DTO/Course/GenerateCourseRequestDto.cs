using System;
using System.Collections.Generic;
using System.Text;

namespace KlikajZGlowa.Service.DTO.Course
{
    public class GenerateCourseRequestDto
    {
        public string Topic { get; set; } = string.Empty;
        public string TargetAudience { get; set; } = "Początkujący";
        public int NumberOfLessons { get; set; } = 3;
    }
}
