using Caspian.Common;
using Warehouse.Model;
using Caspian.Common.Service;

namespace Warehouse.Service
{
    public class GoodsService: BaseService<Goods>
    {
        public GoodsService(IServiceProvider provider)
            :base(provider)
        {
            RuleFor(t => t.BarcodePatternId).Custom(t => t.Code.HasValue() && t.BarcodePatternId.HasValue || !t.Code.HasValue() && t.BarcodePatternId == null, 
                "یکی از مقادیر الگوی کدینگ و کد کالا باید پر باشد");
            RuleFor(t => t.Code).Required().UniqueAsync("کالایی با این کد در سیستم ثبت شده است");
            RuleFor(t => t.Name).Required().UniqueAsync("کالایی با این نام در سیستم ثبت شده است");
        }
    }
}
