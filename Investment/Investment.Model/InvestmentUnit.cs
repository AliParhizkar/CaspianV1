using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Investment.Model
{
    [Table("InvestmentUnits", Schema = "ivm")]
    public class InvestmentUnit
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("کد")]
        public string Code { get; set; }

        [DisplayName("عنوان")]
        public string Title { get; set; }

        [CheckOnDelete("واحد اموال دارای دسترسی می باشد و امکان حذف آن وجود ندارد")]
        public ICollection<InvestmentUnitAccess> InvestmentUnitAccesses { get; set; }
    }
}
