using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Investment.Model
{
    [Table("AccountingCodes", Schema = "acc"), IgnoreTableGeneration]
    public class AccountingCode
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("کد حساب")]
        public string Code { get; set; }

        [DisplayName("نام حساب"), MaxLength(200)]
        public string Name { get; set; }

        [DisplayName("سطح کد حساب")]
        public CodingLevelType CodingLevelType { get; set; }

        [DisplayName("فعال")]
        public bool IsActive { get; set; }

        public int? ParentCodeId { get; set; }

        [ForeignKey(nameof(ParentCodeId))]
        public AccountingCode ParentCode { get; set; }

        [CheckOnDelete("کد دارای زیرمجموعه می باشد و امکان حذف آن وجود ندارد")]
        public ICollection<AccountingCode> DetailsCode { get; set; }

        [CheckOnDelete("کد دارای شرکت بیمه می باشد و امکان حذف آن وجود ندارد")]
        public ICollection<InsuranceCompany> InsuranceCompanies { get; set; }
    }
}
