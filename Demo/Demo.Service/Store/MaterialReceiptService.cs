using System;
using Demo.Model;
using System.Linq;
using Caspian.Common;
using FluentValidation;
using Caspian.Common.Service;

namespace Demo.Service
{
    public class MaterialReceiptService : BaseService<MaterialReceipt>, IBaseService<MaterialReceipt>
    {
        public MaterialReceiptService(IServiceProvider provider)
            : base(provider)
        {
            RuleFor(t => t.QuantityMain).Custom(t => t.QuantityMain == 0 && t.QuantitySub == null, "This parameter must be greater than zero.");
            RuleFor(t => t.MaterialId).Custom(t=> Source != null && Source.Any(u => t.MaterialId == u.MaterialId && u != t), "This item is in the warehouse receipt.");
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
