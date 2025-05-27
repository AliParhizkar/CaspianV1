using Marketing.Model;
using Caspian.Common.Service;

namespace Marketing.Service
{
    public class CustomerAccountingService : BaseService<CustomerAccounting>, IBaseService<CustomerAccounting>
    {
        public CustomerAccountingService(IServiceProvider provider)
            : base(provider)
        {

        }
    }
}
