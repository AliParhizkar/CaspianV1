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

        [DisplayName("First Name")]
        public string FName { get; set; }

        [DisplayName("Last Name")]
        public string LName { get; set; }

        [DisplayName("Gender")]
        public Gender Gender { get; set; }

        [DisplayName("Birth Date ")]
        public DateTime? BirthDate { get; set; }
        
        [DisplayName("سوابق تحصیلی")]
        public IList<CourseStudy> CourseStudies { get; set; }

        [DisplayName("آدرس منزل/کار")]
        public Address Address { get; set; }

        [DisplayName("مشخصات خانوادگی")]
        public Family Family { get; set; }
    }

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

    [Table("Families", Schema = "HR")]
    public class Family
    {
        [Key]
        public int Id { get; set; }

        public string WifeName { get; set; }

        public DateTime BirthDate { get; set; }

        [ForeignKey(nameof(Id))]
        public Employee Employee { get; set; }
    }

    [Table("Addresses", Schema = "HR")]
    public class Address
    {
        [Key]
        public int Id { get; set; }

        public string AddressName { get; set; }

        [ForeignKey(nameof(Id))]
        public Employee Employee { get; set; }

        [DisplayName("Country")]
        public int CountryId { get; set; }

        [ForeignKey(nameof(CountryId))]
        public Country Country { get; set; }
    }
}
