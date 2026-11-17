using System;
using System.Collections.Generic;
using System.Text;
using KlikajZGlowa.Service.DTO.Course;

namespace KlikajZGlowa.Service.DTO.EnrolledUser
{
    public class DetailsEnrolledUserDto
    {
        public int EnrolledUserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<CourseDto> Courses { get; set; }
    }
}
