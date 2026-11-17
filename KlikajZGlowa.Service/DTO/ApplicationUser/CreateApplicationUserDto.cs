using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace KlikajZGlowa.Service.DTO.ApplicationUser
{
    public record CreateApplicationUserDto
    {
        [Required(ErrorMessage = "Imię jest wymagane.")]
        [Display(Name = "Imię")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nazwisko jest wymagane.")]
        [Display(Name = "Nazwisko")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Adres e-mail jest wymagany.")]
        [EmailAddress(ErrorMessage = "Niepoprawny format adresu e-mail.")]
        [Display(Name = "Adres e-mail")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Hasło jest wymagane.")]
        [StringLength(100, ErrorMessage = "{0} musi mieć co najmniej {2} znaków.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Hasło")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Potwierdź hasło")]
        [Compare("Password", ErrorMessage = "Hasła nie są identyczne.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
