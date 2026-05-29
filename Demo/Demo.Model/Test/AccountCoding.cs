using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Demo.Model
{
    [Table("AccountCoding", Schema = "demo")]
    public class AccountCoding
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("کد")]
        public string Code { get; set; }

        [DisplayName("عنوان")]
        public string Title { get; set; }

        [DisplayName("سطح")]
        public int Level { get; set; }

        public int? ParentCodeId { get; set; }

        [ForeignKey(nameof(ParentCodeId))]
        public AccountCoding ParentCode { get; set; }

        public ICollection<AccountCoding> ChildrenCodes { get; set; }
    }
}
