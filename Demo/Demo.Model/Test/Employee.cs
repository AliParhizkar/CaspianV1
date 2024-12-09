using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Demo.Model
{
    [Table("Employees", Schema = "HR")]
    public class Employee
    {
        [Key]
        public int Id { get; set; } 

        public string FName { get; set; }

        public string LName { get; set; }

        [DisplayName("مشخصات خانوادگی")]
        public Family Family { get; set; }

        [DisplayName("سوابق تحصیلی")]
        public virtual IList<CourseStudy> CourseStudies { get; set; }
    }

    [Table("CourseStudies", Schema = "HR")]
    public class CourseStudy
    {
        [Key]
        public int Id { get; set; }

        public string Title { get; set; }

        public int EmployeeId { get; set; }

        [ForeignKey(nameof(EmployeeId))]
        public virtual Employee Employee { get; set; }
    }

    [Table("Family", Schema = "HR")]
    public class Family
    {
        [Key]
        public int Id { get; set; }

        public string WifeName { get; set; }

        public DateTime BirthDate { get; set; }

        [ForeignKey(nameof(Id))]
        public Employee Employee { get; set; }
    }
}
