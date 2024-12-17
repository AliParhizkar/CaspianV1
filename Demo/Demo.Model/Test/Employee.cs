using Caspian.Common;
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

        [DisplayName("حوزه")]
        public int? ScopeId { get; set; }

        [ForeignKey(nameof(ScopeId))]
        public SimpleData Scope { get; set; }

        [DisplayName("مرکز هزینه")]
        public int? CostCenterId { get; set; }

        [ForeignKey(nameof(CostCenterId))]
        public SimpleData CostCenter { get; set; }

        [DisplayName("نوع استخدام")]
        public int? EmploymentTypeId { get; set; }

        [ForeignKey(nameof(EmploymentTypeId))]
        public SimpleData EmploymentType { get; set; }

        [DisplayName("نوع کارمندی")]
        public int? EmployeeTypeId { get; set; }

        [ForeignKey(nameof(EmployeeTypeId))]
        public SimpleData EmployeeType { get; set; }

        [DisplayName("نام")]
        public string FName { get; set; }

        [DisplayName("نام خانوادگی")]
        public string LName { get; set; }

        [DisplayName("تصویر")]
        public byte[] Image { get; set; }

        [DisplayName("جنسیت")]
        public Gender Gender { get; set; }

        [DisplayName("کد ملی")]
        public string IdCard { get; set; }

        [DisplayName("شماره پرونده")]
        public string FileNo { get; set; }

        [DisplayName("شماره مشتخدم")]
        public string EmploymentNo { get; set; }

        [DisplayName("مشخصات شناسنامه ای")]
        public IdentificationDetail IdentificationDetail { get; set; }

        [DisplayName("سوابق تحصیلی"), CheckOnDelete("کارمند دارای سابقه ی تحصیلی می باشد و امکان حذف وی وجود ندارد.")]
        public IList<CourseStudy> CourseStudies { get; set; }

        [DisplayName("آدرس منزل/کار")]
        public Address Address { get; set; }

        [DisplayName("مشخصات خانوادگی")]
        public Family Family { get; set; }

        [DisplayName("دین و مذهب")]
        public ReligionAndSubReligion ReligionAndSubReligion { get; set; }
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

        [DisplayName("نام و نام خانوادگی همسر")]
        public string WifeName { get; set; }

        [DisplayName("تاریخ ازدواج")]
        public DateTime? MariageDate { get; set; }

        [DisplayName("شغل همسر")]
        public int? WifeJobId { get; set; }

        [ForeignKey(nameof(WifeJobId))]
        public SimpleData WifeJob { get; set; }

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

    [Table("ReligionAndSubReligion", Schema = "HR")]
    public class ReligionAndSubReligion
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("دین")]
        public int? ReligionId { get; set; }

        [ForeignKey(nameof(ReligionId))]
        public SimpleData Religion { get; set; }

        [DisplayName("مذهب")]
        public int? SubReligionId { get; set; }

        [ForeignKey(nameof(SubReligionId))]
        public SubReligion SubReligion { get; set; }

        [ForeignKey(nameof(Id))]
        public Employee Employee { get; set; }
    }

    [Table("SubReligions", Schema = "HR")]
    public class SubReligion
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("مذهب")]
        public int ReligionId { get; set; }

        [DisplayName("عنوان")]
        public string Title { get; set; }

        [ForeignKey(nameof(ReligionId))]
        public SimpleData Religion { get; set; }
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
