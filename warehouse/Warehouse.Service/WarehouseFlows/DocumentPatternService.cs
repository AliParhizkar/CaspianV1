using Caspian.Common;
using Warehouse.Model;
using Caspian.Common.Service;

namespace Warehouse.Service
{
    public class DocumentPatternService: BaseService<DocumentPattern>
    {
        public DocumentPatternService(IServiceProvider provider)
            :base(provider)
        {
            RuleFor(t => t.Code).Required().UniqueAsync("الگوی سندی با این کد در سیستم وجود دارد");
            RuleFor(t => t.Title).Required().UniqueAsync("الگوی سندی با این عنوان در سیستم وجود درد");
            RuleFor(t => t.PurchaseType).Required(t => t.DocumentType == DocumentType.Purchase)
                .Custom(t => t.DocumentType != DocumentType.Purchase && t.PurchaseType != null, "مقدار این فیلد باید خالی باشد");
            RuleFor(t => t.RowsCount).Range(10, 500, "مقادیر مجاز بین 10 تا 500 می باشد");
            RuleFor(t => t.ToStockRoomFieldName).Required(t => t.HasToStockRoom)
                .Custom(t => !t.HasToStockRoom && t.ToStockRoomFieldName != null, "مقدار این فیلد باید خالی باشد");
            RuleFor(t => t.ReceiverIssuerFieldName).Required(t => t.HasReceiverIssuer)
                .Custom(t => !t.HasReceiverIssuer && t.ReceiverIssuerFieldName != null, "مقدار این فیلد باید خالی باشد");
            RuleFor(t => t.DocumentRelationshipType)
                .Custom(t => t.DocumentRelationshipType == DocumentRelationshipType.Atff && t.DocumentRelationshipInstance > DocumentRelationshipInstance.RasidEnteghal, "این نوع سند از نوع عطف نمی باشد")
                .Custom(t => t.DocumentRelationshipType == DocumentRelationshipType.Reflex && t.DocumentRelationshipInstance < DocumentRelationshipInstance.BargashtBeKharid, "این نوع سند از نوع برگشتی نمی باشد");
            RuleFor(t => t.DocumentBases).CustomValue(t => t == 0, "حداقل یکی از مقادیر مبناها باید انتخاب شود");
            RuleFor(t => t.DefaultBase).Custom(t => (t.DocumentBases & t.DefaultBase) != t.DefaultBase, "مبنای پیشفرض باید در لیست مبناها باشد");
        }
    }
}
