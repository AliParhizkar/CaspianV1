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

        [DisplayName("مشخصات شناسنامه ای")]
        public IdentificationDetail IdentificationDetail { get; set; }
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
    }

    [Table("IdentificationDetails", Schema = "hr")]
    public class IdentificationDetail
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("نام پدر")]
        public string FatherName { get; set; }

        [DisplayName("شماره شناسنامه")]
        public string IdentificationNo { get; set; }

        [DisplayName("شماره سریال")]
        public string IdentificationSerial { get; set; }

        [DisplayName("تاریخ تولد")]
        public DateTime? BirthDate { get; set; }

        [DisplayName("کشور محل تولد")]
        public int? BirthCountryId { get; set; }

        [DisplayName("استان محل تولد")]
        public int? BirthProvinceId { get; set; }

        [DisplayName("شهر محل تولد")]
        public int? BirthCityId { get; set; }

        [DisplayName("کشور محل صدور")]
        public int? RegCountryId { get; set; }

        [DisplayName("استان محل صدور")]
        public int? RegProvinceId { get; set; }

        [DisplayName("شهر محل صدور")]
        public int? RegCityId { get; set; }

        [ForeignKey(nameof(BirthCountryId))]
        public Country BirthCountry { get; set; }

        [ForeignKey(nameof(BirthProvinceId))]
        public Province BirthProvince { get; set; }

        [ForeignKey(nameof(BirthCityId))]
        public City BirthCity { get; set; }

        [ForeignKey(nameof(RegCountryId))]
        public Country RegCountry { get; set; }

        [ForeignKey(nameof(RegProvinceId))]
        public Province RegProvince { get; set; }

        [ForeignKey(nameof(RegCityId))]
        public City RegCity { get; set; }

        [DisplayName("دهستان")]
        public string Village { get; set; }

        [ForeignKey(nameof(Id))]
        public Employee Employee { get; set; }
    }
}
