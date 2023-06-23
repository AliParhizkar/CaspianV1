using System;
using Demo.Model;
using Caspian.Common;
using Caspian.Common.Service;
using System.Linq;

namespace Demo.Service
{
    public class WareHouseReceiptService : MasterDetailsService<WarehouseReceipt, ReceiptDetail>, IMasterDetailsService<WarehouseReceipt, ReceiptDetail>
    {
        public WareHouseReceiptService(IServiceProvider provider)
            : base(provider)
        {
            RuleFor(t => t.Date).CustomValue(t => t == null, "Please specify the warehouse receipt date.")
                .CustomValue(t => t.HasValue && t.Value.Date > DateTime.Now.Date, "The warehouse receipt date cannot be a future date.");
            RuleFor(t => t.ReceiptDetails).CustomValue(t => t == null || !t.Any(), "Warehouse receipt must have at least one item.");
            RuleForEach(t => t.ReceiptDetails).SetValidator(new ReceiptDetailService(provider));
        }
    }
}
