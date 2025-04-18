using Caspian.Common;
using Marketing.Model;
using Caspian.Common.Service;

namespace Marketing.Service
{
    public class CustomerAddressGroupsService: MasterDetailsService<Customer, CustomerGroupMembership, CustomerAddress>
    {
        public CustomerAddressGroupsService(IServiceProvider provider)
            :base(provider) 
        {
            RuleFor(t => t.Name).Required();
            RuleFor(t => t.MobileNumber).Required().UniqueAsync("مشتری با این شماره همراه در سیستم ثبت شده است");
            RuleForEach(t => t.CustomerGroups).SetValidator(new CustomerGroupMembershipService(provider));
            RuleForEach(t => t.Addresses).SetValidator(new CustomerAddressService(provider));
        }
    }
}
