using System;
using Demo.Model;
using System.Linq;
using Caspian.Common;
using Caspian.Common.Service;

namespace Demo.Service
{
    public class WareHouseReceiptService : MasterDetailsService<WarehouseReceipt, ReceiptDetail>, IMasterDetailsService<WarehouseReceipt, ReceiptDetail>
    {
        public WareHouseReceiptService(IServiceProvider provider)
            : base(provider)
        {
            RuleFor(t => t.Date).CustomValue(t => t == null, "Please specify the warehouse receipt date.")
                .CustomValue(t => t.HasValue && t.Value.Date > DateTime.Now.Date, "The warehouse receipt date cannot be a future date.");
            RuleForEach(t => t.ReceiptDetails).SetValidator(new ReceiptDetailService(provider));
        }
    }
}
