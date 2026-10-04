using System;
using Demo.Model;
using Caspian.Common;
using FluentValidation;
using Caspian.Common.Service;

namespace Demo.Service
{
    public class CustomerGroupService : BaseService<CustomerGroup>
    {
        public CustomerGroupService(IServiceProvider provider)
            :base(provider)
        {
            RuleFor(t => t.Title).Required().UniqueAsync("مشتری با این عنوان در سیستم ثبت شده است");
        }
    }
}
