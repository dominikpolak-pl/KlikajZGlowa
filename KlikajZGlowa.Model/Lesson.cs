using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KlikajZGlowa.Model
{
    public class Lesson
    {
        [Key]
        public int LessonId { get; set; }
        [Display(Name = "Lesson Title")]
        public string Title { get; set; } = string.Empty;
        [Display(Name = "Lesson Content Area")]
        [DataType(DataType.MultilineText)]
        public string ContentArea { get; set; } = string.Empty;

        public int CourseId { get; set; }
        [ForeignKey("CourseId")]
        public Course? Course { get; set; }
    }
}
