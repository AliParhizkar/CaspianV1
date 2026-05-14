using Caspian.Common;
using Warehouse.Model;
using Procurement.Model;
using Warehouse.Service;
using Caspian.Common.Service;

namespace Procurement.Service
{
    public class PurchaseRequestGoodsService: MasterDetailsService<PurchaseRequestGoods, RequestGoodsSupplier>
    {
        public PurchaseRequestGoodsService(IServiceProvider provider)
            :base(provider)
        {
            RuleFor(t => t.GoodsFlowId).Required(t => t.ReferenceDocumentType == ReferenceDocumentType.PurchaseRequest)
                .Custom(t => Source.Any(u => u.Id != t.Id && t.GoodsFlowId == u.GoodsFlowId), "این مبنا قبلا اضافه شده است");
            RuleFor(t => t.ProcurementItemId).Required(t => t.ReferenceDocumentType == ReferenceDocumentType.WithoutReference)
                .Custom(t => Source.Any(u => t.Id != u.Id && t.ProcurementItemId == u.ProcurementItemId), "این قلم خریدنی به لیست اضافه شده است");
            RuleFor(t => t.Amount).CustomAsync(async t =>
            {
                if (t.GoodsFlowId.HasValue)
                {
                    var flow = await provider.GetCaspianService<GoodsFlowService>().SingleAsync(t.GoodsFlowId.Value);
                    if (t.Amount > flow.Quantity)
                        return "تعداد نمی تواند بیشتر از مبناء باشد";
                }
                if (t.Amount <= 0)
                    return "تعداد/مقدار قلم خریدنی باید بزرگتر از صفر باشد";
                return null;
            });
            RuleFor(t => t.NeedDate).Custom(t => t.NeedDate < DateTime.Now.ToDateOnly(), "تاریخ نیاز نمی تواند به تاریخ گذشته باشد");
            
            RuleForEach(t => t.RequestGoodsSuppliers).SetValidator(new RequestGoodsSupplierService(provider));
        }
    }
}
