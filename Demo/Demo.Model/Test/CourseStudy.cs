using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Demo.Model
{
    [Table("CourseStudies", Schema = "HR")]
    public class CourseStudy
    {
        [Key]
        public int Id { get; set; }

        public string Title { get; set; }

        public int EmployeeId { get; set; }

        [ForeignKey(nameof(EmployeeId))]
        public Employee Employee { get; set; }
    }
}
