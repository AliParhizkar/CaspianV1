using Caspian.Common;
using Warehouse.Model;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Procurement.Model
{
    [Table("ProcurementItem", Schema = "pcm")]
    public class ProcurementItem
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("نوع قلم")]
        public ProcurementItemType ProcurementItemType { get; set; }

        [DisplayName("کالا")]
        public int? GoodsId { get; set; }

        [ForeignKey(nameof(GoodsId))]
        public Goods Goods { get; set; }

        [DisplayName("خدمت")]
        public int? ServiceId { get; set; }

        [ForeignKey(nameof(ServiceId))]
        public Service Service { get; set; }

        [DisplayName("فعال")]
        public bool ISActive { get; set; }

        [CheckOnDelete("قلم خریدنی عضو گروه می باشد و امکان حذف آن وجود ندارد")]
        public ICollection<ProcurementItemGrouping> Memberships { get; set; }

        [CheckOnDelete("قلم خریدنی دارای حوزه ی تامین می باشد و امکان حذف آن وجود ندارد")]
        public ICollection<SupplyingScope> SupplyingScopes { get; set; }

        [CheckOnDelete("قلم خریدنی سفارش داده شده و امکان حذف آن وجود ندارد")]
        public ICollection<PurchaseRequestGoods> PurchaseRequestGoods { get; set; }
    }
}
