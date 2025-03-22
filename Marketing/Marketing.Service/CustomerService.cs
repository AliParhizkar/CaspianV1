using Caspian.Common;
using Marketing.Model;
using Caspian.Common.Service;

namespace Marketing.Service
{
    public class CustomerService: MasterDetailsService<Customer, CustomerAddress>, IBaseService<Customer>
    {
        public CustomerService(IServiceProvider provider)
            :base(provider) 
        {
            RuleFor(t => t.Name).Required();
            RuleFor(t => t.MobileNumber).Required().UniqueAsync("مشتری با این شماره همراه در سیستم ثبت شده است");
            RuleForEach(t => t.Addresses).SetValidator(new CustomerAddressService(provider));
        }
    }
}
