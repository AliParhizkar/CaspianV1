using System;
using Demo.Model;
using Caspian.Common;
using Caspian.Common.Service;

namespace Demo.Service
{
    public class CustomerGroupAddressService : MasterDetailsService<Customer, CustomerGroupMembership, CustomerAddress>, 
        IMasterDetailsService<Customer, CustomerGroupMembership, CustomerAddress>
    {
        public CustomerGroupAddressService(IServiceProvider provider)
            : base(provider)
        {
            RuleFor(t => t.CompanyName).Required(t => t.CustomerType == CustomerType.Legal);
            RuleFor(t => t.Gender).Required(t => t.CustomerType == CustomerType.Real);
            RuleFor(t => t.LName).Required(t => t.CustomerType == CustomerType.Real);
            RuleFor(t => t.MobileNumber).Required().MobileNumber().UniqAsync("There is a customer with this mobile number");
            RuleFor(t => t.Tel).TelNumber();

            RuleForEach(t => t.CustomerAddresses).SetValidator(new CustomerAddressService(provider));

            RuleForEach(t => t.CustomerGroupMemberships).SetValidator(new CustomerGroupMembershipService(provider));
        }


    }
}
