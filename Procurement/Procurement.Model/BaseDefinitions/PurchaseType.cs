using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Procurement.Model
{
    [Table("PurchaseTypes", Schema = "pcm")]
    public class PurchaseType
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("کد")]
        public string Code { get; set; }

        [DisplayName("عنوان")]
        public string Title { get; set; }

        [DisplayName("روند خرید")]
        public PurchaseProcessType PurchaseProcessType { get; set; }

        [DisplayName("توضیحات"), MaxLength(200)]
        public string Description { get; set; }

        [DisplayName("فعال")]
        public bool IsActive { get; set; }

        [CheckOnDelete("نوع خرید دارای سفارش خرید می باشد و امکان حذف آن وجود ندارد")]
        public ICollection<PurchaseRequestGoods> PurchaseRequestGoods { get; set; }
    }
}
