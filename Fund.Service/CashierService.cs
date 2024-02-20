using Caspian.Common;
using Caspian.Common.Service;
using Fund.Model;

namespace Fund.Service
{
    public class CashierService : BaseService<Cashier>, IBaseService<Cashier>
    {
        public CashierService(IServiceProvider provider)
            : base(provider)
        {
            RuleFor(t => t.UserId).Required();
            RuleFor(t => t.Status).Required();
            RuleFor(t => t.BeginDate).Required();
            RuleFor(t => t.EndDate).Required();
        }
    }
}
