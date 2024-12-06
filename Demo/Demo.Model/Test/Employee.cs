using System.ComponentModel.DataAnnotations;

namespace Demo.Model
{
    public class Employee
    {
        [Key]
        public int Id { get; set; } 

        public string FName { get; set; }

        public string LName { get; set; }

        public Family Family { get; set; }

        public virtual IList<CourseStudy> CourseStudies { get; set; }
    }

    public class CourseStudy
    {
        [Key]
        public int Id { get; set; }

        public string Title { get; set; }
    }

    public class Family
    {
        [Key]
        public int Id { get; set; }

        public string WifeName { get; set; }

        public DateTime BirthDate { get; set; }
    }
}
