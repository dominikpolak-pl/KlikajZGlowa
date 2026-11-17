using KlikajZGlowa.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace KlikajZGlowa.Service.DTO.AuthorApplication
{
    public class AuthorApplicationDto
    {
        public int Id { get; set; }
        public string ApplicationUserId { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? Motivation { get; set; }
        public DateTime CreatedAt { get; set; }
        public ApplicationStatus Status { get; set; }
        public string? RejectionReason { get; set; }
    }
}
