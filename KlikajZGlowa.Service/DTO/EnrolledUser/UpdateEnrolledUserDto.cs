using System;
using System.Collections.Generic;
using System.Text;

namespace KlikajZGlowa.Service.DTO.EnrolledUser
{
    public class UpdateEnrolledUserDto
    {
        public int EnrolledUserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
