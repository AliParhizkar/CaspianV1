using Accounting.Model;
using Caspian.Engine.Model;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Warehouse.Model
{
    [Table("PurchaseRequests", Schema = "wh")]
    public class PurchaseRequest
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("نوع سند مبناء")]
        public ReferenceDocumentType ReferenceDocumentType { get; set; }

        [DisplayName("نوع طرف مقابل")]
        public OtherPartyType CostCenterType { get; set; }

        [DisplayName("طرف مقابل")]
        public int? CostCenterId { get; set; }

        [ForeignKey(nameof(CostCenterId))]
        public CostCenter CostCenter { get; set; }

        [DisplayName("درخواست کننده")]
        public int RequisitionerId { get; set; }

        [ForeignKey(nameof(RequisitionerId))]
        public User Requisitioner { get; set; }

        [DisplayName("مرکز نگهداری تحویل گیرنده")]
        public int ReceiverKeepCenterId { get; set; }

        [ForeignKey(nameof(ReceiverKeepCenterId))]
        public KeepCenter ReceiverKeepCenter { get; set; }

        [DisplayName("انبار تحویل گیرنده")]
        public int? ReceiverStockRoomId { get; set; }

        [ForeignKey(nameof(ReceiverStockRoomId))]
        public StockRoom ReceiverStockRoom { get; set; }

        [DisplayName("مرکز نگهداری تحویل دهنده")]
        public int IssuerKeepCenterId { get; set; }

        [ForeignKey(nameof(IssuerKeepCenterId))]
        public KeepCenter IssuerKeepCenter { get; set; }

        [DisplayName("انبار تحویل دهنده")]
        public int? IssuerStockRoomId { get; set; }

        [ForeignKey(nameof(IssuerStockRoomId))]
        public StockRoom IssuerStockRoom { get; set; }

        [DisplayName("شماره")]
        public string No { get; set; }

        [DisplayName("تاریخ")]
        public DateOnly Date { get; set; }

        [DisplayName("تاریخ انبار")]
        public DateOnly StockRoomDate { get; set; }

        [DisplayName("توضیحات"), MaxLength(50)]
        public string Description { get; set; }

        [DisplayName("بارگیری دارد")]
        public bool Loadable { get; set; }
    }
}
