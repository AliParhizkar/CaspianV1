using Caspian.Common;
using Marketing.Model;
using Caspian.Common.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

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
            RuleForRemove().CustomAsync(async t =>
            {
                var result = await provider.GetCaspianService<CustomerAddressService>().GetAll().Where(u => u.CustomerId == t.Id).AnyAsync();
                return result;
            }, "مشتری دارای آدرس است و امکان حذف وی وجود ندارد");
            RuleFor(t => t.Address1).Required(t => !ConfigService.Config.CustomerHasManyAddresses);
        }
    }
}
