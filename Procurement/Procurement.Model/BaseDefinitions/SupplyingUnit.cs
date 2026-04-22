using Warehouse.Model;
using Caspian.Engine.Model;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Caspian.Common;

namespace Procurement.Model
{
    [Table("SupplyingUnits")]
    public class SupplyingUnit
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("کد")]
        public string Code { get; set; }

        [DisplayName("عنوان")]
        public string Title { get; set; }

        [DisplayName("شعبه")]
        public int? BranchId { get; set; }

        [ForeignKey(nameof(BranchId))]
        public Branch Branch { get; set; }

        [DisplayName("مدیر/سرپرست")]
        public int? ManagerId { get; set; }

        [ForeignKey(nameof(ManagerId))]
        public User Manager { get; set; }

        [MaxLength, DisplayName("توضیحات")]
        public string Description { get; set; }

        public int? ParentId { get; set; }

        [ForeignKey(nameof(ParentId))]
        public SupplyingUnit Parent { get; set; }

        [CheckOnDelete("واحد تامین دارای عضو(اعضاء) می باشد و امکان حذف آن وجود ندارد")]
        public ICollection<SupplyingUnitMembership> SupplyingUnitMemberships { get; set; }

        [CheckOnDelete("واحد تامین دارای زیرمجموعه می باشد و امکان حذف آن وجود ندارد")]
        public ICollection<SupplyingUnit> Units { get; set; }
    }
}
