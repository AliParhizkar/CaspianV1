using Caspian.Common;
using Marketing.Model;
using Caspian.Common.Service;
using Microsoft.EntityFrameworkCore;

namespace Marketing.Service
{
    public class CustomerGroupsService: MasterDetailsService<Customer, CustomerGroupMembership>, IBaseService<Customer>
    {
        public CustomerGroupsService(IServiceProvider provider)
            :base(provider) 
        {
            
            RuleFor(t => t.Name).Required();
            RuleFor(t => t.MobileNumber).Required().UniqueAsync("مشتری با این شماره همراه در سیستم ثبت شده است").MobileNumber();
            RuleFor(t => t.CustomerNumber).Required().UniqueAsync("مشتری با این شماره در سیستم تعریف شده است")
                .CustomValue(t => t <= 0, "شماره مشتری باید بزرگتر از صفر باشد");

            RuleForEach(t => t.CustomerGroups).SetValidator(new CustomerGroupMembershipService(provider));
            //RuleForRemove().CustomAsync(async t =>
            //{
            //    var result = await provider.GetCaspianService<CustomerGroupMembershipService>().GetAll().Where(u => u.CustomerId == t.Id).AnyAsync();
            //    return result;
            //}, "مشتری عضو گروه است و امکان حذف وی وجود ندارد");
            RuleFor(t => t.Address1).Required();
        }
    }
}
