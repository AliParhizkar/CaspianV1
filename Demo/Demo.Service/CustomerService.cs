using System;
using Demo.Model;
using System.Linq;
using Caspian.Common;
using Caspian.Engine;
using FluentValidation;
using Caspian.Common.Service;
using System.Threading.Tasks;

namespace Demo.Service
{
    [ReportClass]
    public class CustomerService : MasterDetailsService<Customer, CustomerGroupMembership>, IMasterDetailsService<Customer, CustomerGroupMembership>
    {
        public CustomerService(IServiceProvider provider)
            :base(provider)
        {
            RuleFor(t => t.CompanyName).Required(t => t.CustomerType == CustomerType.Legal);
            RuleFor(t => t.Gender).Required(t => t.CustomerType == CustomerType.Real);
            RuleFor(t => t.LName).Required(t => t.CustomerType == CustomerType.Real);
            RuleFor(t => t.MobileNumber).Required().MobileNumber().UniqAsync("There is a customer with this mobile number");
            RuleFor(t => t.Tel).TelNumber();
            RuleFor(t => t.CustomerGroupMemberships).Custom(t => t.CustomerGroupMemberships == null || t.CustomerGroupMemberships.Count == 0, "Customer should be member of a group");
            RuleForEach(t => t.CustomerGroupMemberships).SetValidator(new CustomerGroupMembershipService(provider));
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


        public IQueryable<Customer> GetCustomer(Customer customer)
        {
            return GetAll();
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
