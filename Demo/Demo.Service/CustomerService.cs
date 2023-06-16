using System;
using Demo.Model;
using Caspian.Common;
using Caspian.Engine;
using Caspian.Common.Service;
using System.Threading.Tasks;

namespace Demo.Service
{
    [ReportClass]
    public class CustomerService : BaseService<Customer>, IBaseService<Customer>
    {
        public CustomerService(IServiceProvider provider)
            :base(provider)
        {
            RuleFor(t => t.CompanyName).Required(t => t.CustomerType == CustomerType.Legal);
            RuleFor(t => t.Gender).Required(t => t.CustomerType == CustomerType.Real);
            RuleFor(t => t.LName).Required(t => t.CustomerType == CustomerType.Real);
            RuleFor(t => t.MobileNumber).Required().MobileNumber().UniqAsync("There is a customer with this mobile number");
            RuleFor(t => t.Tel).TelNumber();
        }

        void UpdateCustomer(Customer entity)
        {
            if (entity.CustomerType == CustomerType.Real)
                entity.CompanyName = null;
            else
            {
                entity.FName = null;
                entity.LName = null;
                entity.Gender = null;
            }
        }

        public override Task<Customer> AddAsync(Customer entity)
        {
            UpdateCustomer(entity);
            return base.AddAsync(entity);
        }

        public override Task UpdateAsync(Customer entity)
        {
            UpdateCustomer(entity);
            return base.UpdateAsync(entity);
        }
    }
}
