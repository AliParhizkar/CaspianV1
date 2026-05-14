using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Procurement.Model
{
    [Table("UrgentPurchases", Schema = "pcm")]
    public class UrgentPurchase
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("عنوان")]
        public string Title { get; set; }

        [DisplayName("فعال")]
        public bool IsActive { get; set; }

        [CheckOnDelete("فوریت خرید در درخواست کالا مورد استفاده قرار گرفته و امکان حذف آن وجود ندارد")]
        public ICollection<PurchaseRequestGoods> Goods { get; set; }
    }
}
