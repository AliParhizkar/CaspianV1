using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Warehouse.Model
{
    [Table("DocumentPatterns", Schema = "wh")]
    public class DocumentPattern
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("کد")]
        public string Code { get; set; }

        [DisplayName("عنوان")]
        public string Title { get; set; }

        [DisplayName("نوع سند")]
        public DocumentType DocumentType { get; set; }

        [DisplayName("نوع خرید")]
        public PurchaseType? PurchaseType { get; set; }

        [DisplayName("ورودی/خروجی")]
        public InOutType InOutType { get; set; }

        [DisplayName("نوع تاثیر بر موجودی")]
        public InventoryImpactType InventoryImpactType { get; set; }

        [DisplayName("تعداد سطر")]
        public int RowsCount { get; set; }

        [DisplayName("طرف انبار")]
        public string FromStockRoomFieldName { get; set; }

        [DisplayName("انبار مقابل دارد")]
        public bool HasToStockRoom { get; set; }

        [DisplayName("انبار مقابل")]
        public string ToStockRoomFieldName { get; set; }

        [DisplayName("تحویل دهنده/گیرنده دارد")]
        public bool HasReceiverIssuer { get; set; }

        [DisplayName("عنوان تحویل گیرنده/دهنده")]
        public string ReceiverIssuerFieldName { get; set; }

        [DisplayName("نوع ارتباط سند")]
        public DocumentRelationshipType DocumentRelationshipType { get; set; }

        [DisplayName("نمونه ارتباط سند")]
        public DocumentRelationshipInstance DocumentRelationshipInstance { get; set; }

        [DisplayName("وضعیت")]
        public ActiveStatus ActiveStatus { get; set; }

        [DisplayName("مبنا ها")]
        public DocumentBases DocumentBases { get; set; }

        [DisplayName("مبنای پیشفرض")]
        public DocumentBases DefaultBase { get; set; }

        [DisplayName("نوع کالا")]
        public GoodsType GoodsType { get; set; }

        [DisplayName("طرف مقابل دارد")]
        public bool HasOtherParty { get; set; }

        [DisplayName("شماره قرارداد دارد")]
        public bool HasContractNo { get; set; }

        [CheckOnDelete("الگوی دارای سند می باشد و امکان حذف آن وجود ندارد")]
        public ICollection<StockFlow> StockFlows { get; set; } 
    }
}
