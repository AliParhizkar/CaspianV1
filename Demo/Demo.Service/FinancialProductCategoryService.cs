using Demo.Model;
using Caspian.Common;
using Caspian.Common.Service;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Demo.Service
{
    public class FinancialProductCategoryService : SimpleService<FinancialProductCategory>, ISimpleService<FinancialProductCategory>
    {
        public FinancialProductCategoryService(IServiceProvider provider)
            : base(provider)
        {
            RuleFor(t => t.Title).Required().UniqAsync("گروه محصولی با این عنوان تعریف شده است");
        }
    }
}
