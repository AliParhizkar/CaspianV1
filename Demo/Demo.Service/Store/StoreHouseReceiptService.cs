using System;
using Demo.Model;
using Caspian.Common;
using FluentValidation;
using Caspian.Common.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Demo.Service
{
    public class StoreHouseReceiptService : SimpleService<StoreHouseReceipt>, ISimpleService<StoreHouseReceipt>
    {
        public StoreHouseReceiptService(IServiceScope scope)
            : base(scope)
        {
            RuleFor(t => t.Date).CustomValue(t => t == null, "تاریخ حواله باید مشخص باشد")
                .CustomValue(t => t.HasValue && t.Value.Date > DateTime.Now.Date, "حواله نمی تواند به تاریخ آینده باشد.");
            RuleForEach(t => t.MaterialReceipts).SetValidator(new MaterialReceiptService(scope))
                .When((t) => 
                {
                    return !IgnoreDetailsProperty;
                });
        }
    }
}
