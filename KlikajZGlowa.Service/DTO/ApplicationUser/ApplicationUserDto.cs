using System;
using System.Collections.Generic;
using System.Text;
using KlikajZGlowa.Model;

namespace KlikajZGlowa.Service.DTO.ApplicationUser
{
    public record ApplicationUserDto
    {
        public string Id { get; set; }
        public string FirstName { get; set; } 
        public string LastName { get; set; } 
        public string FullName { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public int? AuthorId { get; set; }
        public int? EnrolledUserId { get; set; }
    }
}
