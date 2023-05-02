using System;
using Demo.Model;
using Caspian.Common;
using FluentValidation;
using Caspian.Common.Service;
using System.Linq;

namespace Demo.Service
{
    public class WareHouseReceiptService : BaseService<WarehouseReceipt>, IBaseService<WarehouseReceipt>
    {
        public WareHouseReceiptService(IServiceProvider provider)
            : base(provider)
        {
            RuleFor(t => t.Date).CustomValue(t => t == null, "تاریخ حواله باید مشخص باشد")
                .CustomValue(t => t.HasValue && t.Value.Date > DateTime.Now.Date, "حواله نمی تواند به تاریخ آینده باشد.");
            RuleFor(t => t.MaterialReceipts).CustomValue(t => t == null || !t.Any(), "حواله باید حداقل یک کالا داشته باشد");
            RuleForEach(t => t.MaterialReceipts).SetValidator(new MaterialReceiptService(provider));
        }
    }
}
