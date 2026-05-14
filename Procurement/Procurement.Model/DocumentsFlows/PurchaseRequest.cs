using Caspian.Common;
using Warehouse.Model;
using Accounting.Model;
using Caspian.Engine.Model;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Procurement.Model
{
    [Table("PurchaseRequests", Schema = "pcm")]
    public class PurchaseRequest  
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("شماره سفارش")]
        public string No { get; set; }

        [DisplayName("تاریخ سفارش")]
        public DateOnly Date { get; set; }

        [DisplayName("نوع قلم")]
        public ProcurementItemType ProcurementItemType { get; set; }

        [DisplayName("م.ن درخواست کننده")]
        public int KeepCenterId { get; set; }

        [ForeignKey(nameof(KeepCenterId))]
        public KeepCenter KeepCenter { get; set; }

        [DisplayName("درخواست کننده")]
        public int? RequesterId { get; set; }

        [ForeignKey(nameof(RequesterId))]
        public User Requester { get; set; }

        [DisplayName("نوع طرف مقابل")]
        public OtherPartyType OtherPartyType { get; set; }

        [DisplayName("طرف مقابل")]
        public int? CostCenterId { get; set; }

        [ForeignKey(nameof(CostCenterId))]
        public CostCenter CostCenter { get; set; }

        [DisplayName("طرف مقابل")]
        public int? SupplierId { get; set; }

        [ForeignKey(nameof(SupplierId))]
        public Supplier Supplier { get; set; }

        [MaxLength(200), DisplayName("توضیحات")]
        public string Description { get; set; }

        [DisplayName("نوع درخواست خرید")]
        public PurchaseRequestType? PurchaseRequestType { get; set; }

        [CheckOnDelete("درخواست دارای کالا می باشد و امکان حذف آن وجود ندارد")]
        public ICollection<PurchaseRequestGoods> Goods { get; set; }
    }
}
