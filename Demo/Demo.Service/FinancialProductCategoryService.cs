using System;
using Demo.Model;
using Caspian.Common;
using Caspian.Common.Service;

namespace Demo.Service
{
    public class FinancialProductCategoryService : BaseService<FinancialProductCategory>
    {
        public FinancialProductCategoryService(IServiceProvider provider)
            : base(provider)
        {
            RuleFor(t => t.Title).Required().UniqueAsync("گروه محصولی با این عنوان تعریف شده است");
        }
    }
}
