using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KlikajZGlowa.Model
{
    public class Course
    {
        [Key]
        public int CourseId { get; set; }
        [Display(Name = "Course Title")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "Course Description")]
        public string Description { get; set; } = string.Empty;

        public ICollection<EnrolledUser> EnrolledUsers { get; set; } = new HashSet<EnrolledUser>();

        public ICollection<Author> Authors { get; set; } = new HashSet<Author>();

        public ICollection<Lesson> Lessons { get; set; } = new HashSet<Lesson>();
    }
}
