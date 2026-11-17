using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KlikajZGlowa.Model
{
    public class Author
    {
        [Key]
        public int AuthorId { get; set; }
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;
        [Display(Name = "Full Name")]
        public string FullName { get => $"{FirstName} {LastName}"; }
        [Display(Name = "Courses: ")]
        public ICollection<Course> Courses { get; set; } = new HashSet<Course>();
        public string ApplicationUserId { get; set; } = null!;
        [ForeignKey("ApplicationUserId")]
        public ApplicationUser? ApplicationUser { get; set; }
    }
}
