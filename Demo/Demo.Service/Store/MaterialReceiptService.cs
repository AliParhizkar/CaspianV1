using System;
using Demo.Model;
using System.Linq;
using Caspian.Common;
using FluentValidation;
using Caspian.Common.Service;
using Caspian.Common.Extension;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Demo.Service
{
    public class ReceiptDetailService : BaseService<ReceiptDetail>, IBaseService<ReceiptDetail>
    {
        public ReceiptDetailService(IServiceProvider provider)
            : base(provider)
        {
            RuleFor(t => t.QuantityMain).Custom(t => t.QuantityMain == 0 && t.QuantitySub == null, "This parameter must be greater than zero.");
            RuleFor(t => t.MaterialId).Custom(t=> Source != null && Source.Any(u => t.MaterialId == u.MaterialId && u != t), "This item is in the warehouse receipt.");
            RuleFor(t => t.QuantitySub).CustomAsync(async t =>
            {
                if (t.MaterialId > 0)
                {

                    var old = await GetService<MaterialService>().GetAll().Include(t => t.Subunit).SingleOrDefaultAsync(t.MaterialId);
                    if (old != null)
                    {
                        if (old.SubunitId.HasValue)
                        {
                            if (t.QuantitySub == null || t.QuantitySub > old.Subunit.Factor)
                                return true;
                        }
                        if (old.SubunitId == null && t.QuantitySub.HasValue)
                            return true;
                    }
                }

                return false;
            }, "The value of sub-unit is invalid").Custom(t => t.QuantityMain == 0 && t.QuantitySub == 0, "This parameter must be greater than zero.");
        }
    }
}
