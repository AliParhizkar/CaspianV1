using System;
using Demo.Model;
using System.Linq;
using Caspian.Common;
using Caspian.Common.Service;
using System.Threading.Tasks;
using Caspian.Common.Extension;
using Microsoft.EntityFrameworkCore;

namespace Demo.Service
{
    public class WareHouseReceiptService : MasterDetailsService<WarehouseReceipt, ReceiptDetail>, IMasterDetailsService<WarehouseReceipt, ReceiptDetail>
    {
        public WareHouseReceiptService(IServiceProvider provider)
            : base(provider)
        {
            RuleFor(t => t.Date).CustomValue(t => t == default, "Please specify the warehouse receipt date.")
                .CustomValue(t => t > DateTime.Now.ToDateOnly(), "The warehouse receipt date cannot be a future date.");
            RuleFor(t => t.ReceiptDetails).Custom(t => DetailService.Source.Count() == 0, "The receipt must contain at least one item.");
        }

        public override async Task<WarehouseReceipt> AddAsync(WarehouseReceipt receipt)
        {
            var date = DateTime.Now.ToDateOnly();
            var maxId = await GetAll().Where(t => t.Date == date).MaxAsync(t => (int?)t.ReceiptNo);
            receipt.ReceiptNo = maxId.GetValueOrDefault() + 1;
            return await base.AddAsync(receipt);
        }

        public override async Task UpdateAsync(WarehouseReceipt entity)
        {
            var old = await SingleAsync(entity.Id);
            entity.ReceiptNo = old.ReceiptNo;
            await base.UpdateAsync(entity);
        }
    }
}
