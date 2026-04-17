using Caspian.Common;
using Accounting.Model;
using Caspian.Engine.Model;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Warehouse.Model
{
    [Table("StockFlow", Schema = "wh")]
    public class StockFlow
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("مرکز نگهداری")]
        public int KeepCenterId { get; set; }

        [ForeignKey(nameof(KeepCenterId))]
        public KeepCenter KeepCenter { get; set; }

        [DisplayName("الگوی سند")]
        public int? DocumentPatternId { get; set; }

        [ForeignKey(nameof(DocumentPatternId))]
        public DocumentPattern DocumentPattern { get; set; }

        [DisplayName("انبار")]
        public int? StockRoomId { get; set; }

        [ForeignKey(nameof(StockRoomId))]
        public StockRoom StockRoom { get; set; }

        [DisplayName("مرکز نگهداری مقابل")]
        public int? OtherKeepCenterId { get; set; }

        [ForeignKey(nameof(OtherKeepCenterId))]
        public KeepCenter OtherKeepCenter { get; set; }

        [DisplayName("انبار مقابل")]
        public int? OtherStockRoomId { get; set; }

        [ForeignKey(nameof(OtherStockRoomId))]
        public StockRoom OtherStockRoom { get; set; }

        [DisplayName("نوع طرف مقابل")]
        public OtherPartyType OtherPartyType { get; set; }

        [DisplayName("مرکز هزینه")]
        public int? CostCenterId { get; set; }

        [ForeignKey(nameof(CostCenterId))]
        public CostCenter CostCenter { get; set; }

        [DisplayName("تامین کننده")]
        public int? SupplierId { get; set; }

        [ForeignKey(nameof(SupplierId))]
        public Supplier Supplier { get; set; }

        [DisplayName("شماره")]
        public string No { get; set; }

        [DisplayName("شماره قرارداد")]
        public string ContractNo { get; set; }

        [DisplayName("تاریخ")]
        public DateOnly Date { get; set; }

        [DisplayName("شعبه")]
        public string Branch { get; set; }

        [MaxLength(200), DisplayName("توضیحات")]
        public string Description { get; set; }

        [DisplayName("معین طرف مقابل")]
        public int? AccountingCodeId { get; set; }

        [ForeignKey(nameof(AccountingCodeId))]
        public AccountingCode AccountingCode { get; set; }

        [DisplayName("نوع گردش")]
        public StockFlowType StockFlowType { get; set; }

        [CheckOnDelete("گردش دارای کالا می باشد و امکان حذف آن وجود ندارد")]
        public ICollection<GoodsFlow> GoodsFlows { get; set; }

        #region Goods Request Fields
        [DisplayName("نوع سند مبناء")]
        public ReferenceDocumentType? ReferenceDocumentType { get; set; }

        [DisplayName("درخواست کننده")]
        public int? RequisitionerId { get; set; } 

        [ForeignKey(nameof(RequisitionerId))]
        public User Requisitioner { get; set; }

        [DisplayName("تاریخ انبار")]
        public DateOnly? StockRoomDate { get; set; }

        [DisplayName("بارگیری دارد")]
        public bool Loadable { get; set; } 
        #endregion
    }
}
