using Caspian.Common;
using Warehouse.Model;
using Caspian.Common.Service;

namespace Warehouse.Service
{
    public class ReceiptService : MasterDetailsService<Receipt, ReceiptDetails>
    {
        public ReceiptService(IServiceProvider provider)
            :base(provider)
        {
            RuleFor(t => t.No).Required();
            RuleForEach(t => t.ReceiptDetails).SetValidator(new ReceiptDetailsService(provider));
        }
    }
}
