using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Procurement.Model
{
    [Table("RequestGoodsSuppliers", Schema = "pcm")]
    public class RequestGoodsSupplier
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("کالای درخواستی")]
        public int PurchaseRequestGoodsId { get; set; }

        [ForeignKey(nameof(PurchaseRequestGoodsId))]
        public PurchaseRequestGoods PurchaseRequestGoods { get; set; }

        [DisplayName("تامین کننده")]
        public int SupplierId { get; set; }

        [ForeignKey(nameof(SupplierId))]
        public Supplier Supplier { get; set; }
    }
}
