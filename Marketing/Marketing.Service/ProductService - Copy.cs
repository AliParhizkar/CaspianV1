using Marketing.Model;
using Caspian.Common.Service;

namespace Marketing.Service
{
    public class CustomerGroupMemberShipService : BaseService<CustomerGroupMemberShip>, IBaseService<CustomerGroupMemberShip>
    {
        public CustomerGroupMemberShipService(IServiceProvider provider)
            : base(provider)
        {

        }
    }
}
