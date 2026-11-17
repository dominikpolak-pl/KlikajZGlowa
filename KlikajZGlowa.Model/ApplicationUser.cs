using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace KlikajZGlowa.Model
{
    public class ApplicationUser:IdentityUser
    {
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;
        [Display(Name = "Full Name")]
        public string FullName { get => $"{FirstName} {LastName}"; } 

        [Display(Name = "Enrolled User")]
        public EnrolledUser? EnrolledUser { get; set; }
        [Display(Name = "Author")]
        public Author? Author { get; set; }
    }
}
