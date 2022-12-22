using Demo.Model;
using Caspian.Common;
using Caspian.Common.Service;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Demo.Service
{
    public class MaterialReceiptService : SimpleService<MaterialReceipt>, ISimpleService<MaterialReceipt>
    {
        public MaterialReceiptService(IServiceProvider provider)
            : base(provider)
        {
            RuleFor(t => t.QuantityMain).CustomValue(t => t <= 0, "واحد اصلی باید بزرگتر از صفر باشد");
            RuleFor(t => t.MaterialId).UniqAsync(t=> t.ReceiptId, "این محصول در حواله وجود دارد");
            RuleFor(t => t.QuantitySub).CustomAsync(async t =>
            {
                if (t.MaterialId > 0)
                {
                    var old = await new MaterialService(provider).SingleOrDefaultAsync(t.MaterialId);
                    if (old != null)
                    {
                        if (old.SubunitId.HasValue && t.QuantitySub == null)
                            return true;
                        if (old.SubunitId == null && t.QuantitySub.HasValue)
                            return true;
                    }
                }
                return false;
            }, "مقدار واحد فرعی نامعتبر است");
        }
    }
}
