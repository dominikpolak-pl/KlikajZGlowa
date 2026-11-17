using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KlikajZGlowa.Model
{
    public class EnrolledUser
    {
        [Key]
        public int EnrolledUserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string FullName { get => $"{FirstName} {LastName}"; }
        public string LastName { get; set; } = string.Empty;
        public string ApplicationUserId { get; set; } = null!;
        [ForeignKey("ApplicationUserId")]
        public ApplicationUser? ApplicationUser { get; set; }
        public ICollection<Course> Courses { get; set; } = new HashSet<Course>();
    }
}
