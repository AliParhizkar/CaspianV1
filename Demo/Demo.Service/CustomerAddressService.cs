using System;
using Demo.Model;
using Caspian.Common;
using Caspian.Common.Service;
using System.Threading.Tasks;

namespace Demo.Service
{
    public class CustomerAddressService : BaseService<CustomerAddress>, IBaseService<CustomerAddress>
    {
        public CustomerAddressService(IServiceProvider provider)
            : base(provider)
        {
            RuleFor(t => t.Address).Required();
        }

        public override async Task<CustomerAddress> AddAsync(CustomerAddress entity)
        {
            entity.IsDefault = true;
            entity.IsActive = true;
            return await base.AddAsync(entity);
        }
    }
}
