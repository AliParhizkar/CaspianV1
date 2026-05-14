using Caspian.Common;
using Caspian.Engine.Model;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Procurement.Model
{
    [Table("PurchasingSpecialist", Schema = "pcm")]
    public class PurchasingSpecialist
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("کد")]
        public string Code { get; set; }

        [DisplayName("کاربر")]
        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; }

        [DisplayName("فعال")]
        public bool IsActive { get; set; }

        [MaxLength(200), DisplayName("توضیحات")]
        public string Description { get; set; }

        [CheckOnDelete("کارشناس عضو واحد تامین می باشد و امکان حذف آن وجود ندارد")]
        public ICollection<SupplyingUnitMembership> SupplyingUnitMemberships { get; set; }

        [CheckOnDelete("کارشناس خرید دارای درخواست کالا می باشد و امکان حذف وی وجود ندارد")]
        public ICollection<PurchaseRequestGoods> PurchasingSpecialists { get; set; }
    }
}
