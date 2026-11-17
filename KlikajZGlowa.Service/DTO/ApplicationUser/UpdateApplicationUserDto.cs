using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace KlikajZGlowa.Service.DTO.ApplicationUser
{
    public record UpdateApplicationUserDto
    {
        [Required(ErrorMessage = "Imię jest wymagane.")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nazwisko jest wymagane.")]
        public string LastName { get; set; } = string.Empty;

        public string? PhoneNumber { get; set; }
    }
}
