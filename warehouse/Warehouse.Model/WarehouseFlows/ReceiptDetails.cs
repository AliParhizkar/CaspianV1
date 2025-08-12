using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Warehouse.Model
{
    [Table("ReceiptDetails", Schema = "wh")]
    public class ReceiptDetails
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("رسید انبار")]
        public int ReceiptId { get; set; }

        [ForeignKey(nameof(ReceiptId))]
        public Receipt Receipt { get; set; }

        [DisplayName("کالا")]
        public int ProductId { get; set; }

        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; }

        [DisplayName("تعداد/مقدار")]
        public decimal Quantity { get; set; }

        [DisplayName("مبلغ")]
        public decimal Price { get; set; }

        [DisplayName("تاریخ انقضاء")]
        public DateOnly  ExpireDate { get; set; }

        [DisplayName("شرح")]
        public string Description { get; set; }
    }
}
