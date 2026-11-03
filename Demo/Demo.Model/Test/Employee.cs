using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Demo.Model
{
    [Table("Employees", Schema = "demo")]
    public class Employee
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("Department")]
        public int ScopeId { get; set; }

        [ForeignKey(nameof(ScopeId))]
        public SimpleData Scope { get; set; }

        [DisplayName("Cost Center")]
        public int? CostCenterId { get; set; }

        [ForeignKey(nameof(CostCenterId))] 
        public SimpleData CostCenter { get; set; }

        [DisplayName("Employment Type")]
        public int? EmploymentTypeId { get; set; }

        [ForeignKey(nameof(EmploymentTypeId))]
        public SimpleData EmploymentType { get; set; }

        [DisplayName("Employee Category")]
        public int? EmployeeTypeId { get; set; }

        [ForeignKey(nameof(EmployeeTypeId))]
        public SimpleData EmployeeType { get; set; }

        [DisplayName("First Name")]
        public string FName { get; set; }

        [DisplayName("Last Name")]
        public string LName { get; set; }

        [DisplayName("Image")]
        public byte[] Image { get; set; }

        [DisplayName("Gender")]
        public Gender Gender { get; set; }

        [DisplayName("National ID")]
        public string IdCard { get; set; }

        [DisplayName("Personnel File Number")]
        public string FileNo { get; set; }

        [DisplayName("Employee Number")]
        public string EmploymentNo { get; set; }

        [DisplayName("Personal Identification")]
        public IdentificationDetail IdentificationDetail { get; set; } 

        [DisplayName("Educational Background"), CheckOnDelete("The employee has educational records and cannot be deleted.")]
        public IList<CourseStudy> CourseStudies { get; set; }

        [DisplayName("Home/Work Address")]
        public Address Address { get; set; }

        [DisplayName("Family Information")]
        public Family Family { get; set; }

        [DisplayName("Religion")]
        public ReligionAndSubReligion ReligionAndSubReligion { get; set; }
    }

    [Table("Families", Schema = "demo")]
    public class Family
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("Spouse's Full Name")]
        public string WifeName { get; set; }

        [DisplayName("Marriage Date")]
        public DateOnly? MariageDate { get; set; }

        [DisplayName("Spouse's Occupation")]
        public int? WifeJobId { get; set; }

        [ForeignKey(nameof(WifeJobId))]
        public SimpleData WifeJob { get; set; }

        [ForeignKey(nameof(Id))]
        public Employee Employee { get; set; }
    }

    [Table("Addresses", Schema = "demo")]
    public class Address
    {
        [Key]
        public int Id { get; set; }

        public string AddressName { get; set; }

        [ForeignKey(nameof(Id))]
        public Employee Employee { get; set; }
    }

    [Table("IdentificationDetails", Schema = "demo")]
    public class IdentificationDetail
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("Father's Name")]
        public string FatherName { get; set; }

        [DisplayName("Birth Certificate Number")]
        public string IdentificationNo { get; set; }

        [DisplayName("Serial Number")]
        public string IdentificationSerial { get; set; }

        [DisplayName("Date of Birth")]
        public DateOnly? BirthDate { get; set; }

        [DisplayName("Country of Birth")]
        public int? BirthCountryId { get; set; }

        [DisplayName("Province of Birth")]
        public int? BirthProvinceId { get; set; }

        [DisplayName("City of Birth")]
        public int? BirthCityId { get; set; }

        [DisplayName("Country of Issue")]
        public int? RegCountryId { get; set; }

        [DisplayName("Province of Issue")]
        public int? RegProvinceId { get; set; }

        [DisplayName("City of Issue")]
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

        [DisplayName("Rural District")]
        public string Village { get; set; }

        [ForeignKey(nameof(Id))]
        public Employee Employee { get; set; }
    }

    [Table("ReligionAndSubReligion", Schema = "demo")]
    public class ReligionAndSubReligion
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("Religion")]
        public int? ReligionId { get; set; }

        [ForeignKey(nameof(ReligionId))]
        public SimpleData Religion { get; set; }

        [DisplayName("Denomination")]
        public int? SubReligionId { get; set; }

        [ForeignKey(nameof(SubReligionId))]
        public SubReligion SubReligion { get; set; }

        [ForeignKey(nameof(Id))]
        public Employee Employee { get; set; }
    }

    [Table("SubReligions", Schema = "demo")]
    public class SubReligion
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("Religion")]
        public int ReligionId { get; set; }

        [DisplayName("Title")]
        public string Title { get; set; }

        [ForeignKey(nameof(ReligionId))]
        public SimpleData Religion { get; set; }

        [CheckOnDelete("کارمندی با این مذهب ثبت شده و امکان حذف آن وجود ندارد")]
        public ICollection<ReligionAndSubReligion> ReligionAndSubReligions { get; set; }
    }
}
