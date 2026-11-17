using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace KlikajZGlowa.Service.DTO.ApplicationUser
{
    public record LoginApplicationUserDto
    {
        [Required(ErrorMessage = "Adres e-mail jest wymagany.")]
        [EmailAddress(ErrorMessage = "Niepoprawny format adresu e-mail.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Hasło jest wymagane.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }
    }
}
