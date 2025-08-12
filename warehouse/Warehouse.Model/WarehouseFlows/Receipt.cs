using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Warehouse.Model
{
    [Table("Receipt", Schema = "wh")]
    public class Receipt
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("شماره رسید")]
        public string No { get; set; }

        [DisplayName("تاریخ رسید")]
        public DateOnly? ReceiptDate { get; set; }

        [DisplayName("فروشنده")]
        public int? SellerId { get; set; }

        [ForeignKey(nameof(SellerId))]
        public Seller Seller { get; set; }

        [DisplayName("کارپرداز")]
        public int? SupplierId { get; set; }

        [ForeignKey(nameof(SellerId))]
        public Supplier Supplier { get; set; }

        [DisplayName("نوع سند")]
        public ReceiptType? ReceiptType { get; set; }

        [DisplayName("شرح")]
        public string Description { get; set; }

        [CheckOnDelete("رسید دارای محصول می باشد و امکان حذف آن وجود ندارد")]
        public ICollection<ReceiptDetails> ReceiptDetails { get; set; }
    }
}
