using Accounting.Model;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Investment.Model
{
    [Table("InsuranceCompanies", Schema = "ivm")]
    public class InsuranceCompany
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("کد بیمه")]
        public string Code { get; set; }

        [DisplayName("نام بیمه")]
        public string Name { get; set; }

        [ForeignKey(nameof(AccountingCodeId))]
        public AccountingCode AccountingCode { get; set; }

        [DisplayName("کد حسابداری")]
        public int AccountingCodeId { get; set; }

        [DisplayName("نوع")]
        public InsuranceCompanyType InsuranceCompanyType { get; set; }

        [DisplayName("شناسه ملی")]
        public string NationalCode { get; set; }

        [DisplayName("کد اقتصادی")]
        public string EconomicCode { get; set; }

        [DisplayName("استان")]
        public int ProvinceId { get; set; }

        [ForeignKey(nameof(ProvinceId))]
        public Location Province { get; set; }

        [DisplayName("شهر")]
        public int? CityId { get; set; }

        [ForeignKey(nameof(CityId))]
        public Location City { get; set; }

        [DisplayName("شماره تلفن")]
        public string Tell { get; set; }

        [DisplayName("شماره نمابر")]
        public string Fax { get; set; }

        [DisplayName("آدرس الکترونیکی"), MaxLength(100)]
        public string Email { get; set; }

        [DisplayName("آدرس"), MaxLength(200)]
        public string Address { get; set; }

        [DisplayName("توضیحات"), MaxLength(200)]
        public string Description { get; set; }
    }
}
