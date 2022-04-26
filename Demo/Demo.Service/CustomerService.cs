using Demo.Model;
using Caspian.Common;
using Caspian.Engine;
using Caspian.Common.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Demo.Service
{
    [ReportClass]
    public class CustomerService : SimpleService<Customer>, ISimpleService<Customer>
    {
        public CustomerService(IServiceScope scope)
            :base(scope)
        {
            RuleFor(t => t.CompanyName).Required(t => t.CustomerType == CustomerType.Legal);
            RuleFor(t => t.Gender).Required(t => t.CustomerType == CustomerType.Real);
            RuleFor(t => t.LName).Required(t => t.CustomerType == CustomerType.Real);
            RuleFor(t => t.MobileNumber).Required().MobileNumber();
            RuleFor(t => t.Tel).TelNumber();
        }
    }
}
