using Caspian.Common;
using Marketing.Model;
using Caspian.Common.Service;

namespace Marketing.Service
{
    public class CustomerAddressService : BaseService<CustomerAddress>, IBaseService<CustomerAddress>
    {
        public CustomerAddressService(IServiceProvider provider)
            : base(provider)
        {
            RuleFor(t => t.Address).Required();
            //RuleFor(t => t.IsDefault).Custom(t => 
            //{
            //    var result = t.IsDefault && Source.Any(u => u.IsDefault & t.Id != u.Id);
            //    return result;
            //}, "فقط یک آدرس می تواند بعنوان آدرس پیش فرض باشد");
        }
    }
}
