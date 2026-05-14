using Warehouse.Model;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Caspian.Common;

namespace Procurement.Model
{
    [Table("PurchaseRequestsGoods", Schema = "pcm")]
    public class PurchaseRequestGoods
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("درخواست کالا")]
        public int PurchaseRequestId { get; set; }

        [ForeignKey(nameof(PurchaseRequestId))]
        public PurchaseRequest PurchaseRequest { get; set; }

        [DisplayName("نوع مبناء")]
        public ReferenceDocumentType ReferenceDocumentType { get; set; }

        [DisplayName("مبناء")]
        public int? GoodsFlowId { get; set; }

        [ForeignKey(nameof(GoodsFlowId))]
        public GoodsFlow GoodsFlow { get; set; }

        [DisplayName("قلم خریدنی")]
        public int? ProcurementItemId { get; set; }

        [ForeignKey(nameof(ProcurementItemId))]
        public ProcurementItem ProcurementItem { get; set; }

        [DisplayName("مقدار")]
        public decimal Amount { get; set; }

        [DisplayName("تاریخ نیاز")]
        public DateOnly? NeedDate { get; set; }

        [DisplayName("فوریت خرید")]
        public int? UrgentPurchaseId { get; set; }

        [ForeignKey(nameof(UrgentPurchaseId))]
        public UrgentPurchase UrgentPurchase { get; set; }

        [MaxLength(200), DisplayName("توضیحات")]
        public string Description { get; set; }

        [DisplayName("واحد تامین")]
        public int? SupplyingUnitId { get; set; }

        [ForeignKey(nameof(SupplyingUnitId))]
        public SupplyingUnit SupplyingUnit{ get; set; }

        [DisplayName("نوع خرید")]
        public int? PurchaseTypeId { get; set; }

        [ForeignKey(nameof(PurchaseTypeId))]
        public PurchaseType PurchaseType { get; set; }

        [DisplayName("مهلت استعلام")]
        public DateOnly? InquiryDeadline { get; set; }

        [DisplayName("تامین کننده")]
        public int? SupplierId { get; set; }

        [ForeignKey(nameof(SupplierId))]
        public Supplier Supplier { get; set; }

        [CheckOnDelete("درخواست کالا دارای تامین کننده می باشد و امکان حذف آن وجود ندارد")]
        public ICollection<RequestGoodsSupplier> RequestGoodsSuppliers { get; set; }

        [DisplayName("کارشناس خرید")]
        public int? PurchasingSpecialistId { get; set; }

        [ForeignKey(nameof(PurchasingSpecialistId))]
        public PurchasingSpecialist PurchasingSpecialist { get; set; }
    }
}
