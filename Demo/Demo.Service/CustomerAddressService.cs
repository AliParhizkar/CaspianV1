using System;
using Demo.Model;
using Caspian.Common;
using Caspian.Common.Service;
using System.Threading.Tasks;

namespace Demo.Service
{
    public class CustomerAddressService : DetailService<CustomerAddress>
    {
        public CustomerAddressService(IServiceProvider provider)
            : base(provider)
        {
            RuleFor(t => t.Address).Required();
        }

        public override async Task AddAsync(CustomerAddress entity)
        {
            entity.IsDefault = true;
            entity.IsActive = true;
            await base.AddAsync(entity);
        }
    }
}
