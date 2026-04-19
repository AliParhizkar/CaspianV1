using Caspian.Common;
using Warehouse.Model;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Procurement.Model
{
    [Table("SupplierEvaluations", Schema = "pcm")]
    public class SupplierEvaluation
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("شماره")]
        public string No { get; set; }

        [DisplayName("تامین کننده")]
        public int SupplierId { get; set; }

        [ForeignKey(nameof(SupplierId))]
        public Supplier Supplier { get; set; }

        [DisplayName("نوع مبناء")]
        public EvaluationBaseType? EvaluationBaseType { get; set; }

        [DisplayName("رسید انبار")]
        public int? ReceiptId { get; set; }

        [ForeignKey(nameof(ReceiptId))]
        public StockFlow Receipt { get; set; }

        [DisplayName("رتبه")]
        public SupplierRank SupplierRank { get; set; }

        [DisplayName("تاریخ شروع اعتبار")]
        public DateOnly? StartDate { get; set; }

        [DisplayName("تاریخ پایان اعتبار")]
        public DateOnly? EndDate { get; set; }

        [DisplayName("تاریخ ارزیابی")]
        public DateOnly? Date { get; set; }

        [CheckOnDelete("این ارزیابی دارای تامین کننده می باشد و امکان حذف آن وجود ندارد")]
        public ICollection<SupplierEvaluationDetail> Details { get; set; }
    }
}
