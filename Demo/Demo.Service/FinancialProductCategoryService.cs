using Demo.Model;
using Caspian.Common;
using Caspian.Common.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Demo.Service
{
    public class FinancialProductCategoryService : SimpleService<FinancialProductCategory>, ISimpleService<FinancialProductCategory>
    {
        public FinancialProductCategoryService(IServiceScope scope)
            : base(scope)
        {
            RuleFor(t => t.Title).Required().UniqAsync("گروه محصولی با این عنوان تعریف شده است");
        }
    }
}
