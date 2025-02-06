using Caspian.Common;
using Marketing.Model;
using Caspian.Common.Service;

namespace Marketing.Service
{
    public class ProductCategoryService : BaseService<ProductCategory>, IBaseService<ProductCategory>
    {
        public ProductCategoryService(IServiceProvider provider)
            : base(provider)
        {
            RuleFor(t => t.Title).Required().UniqueAsync("The title of the product category must be unique.");
            RuleFor(t => t.Code).UniqueAsync("The code of the product category must be unique.")
            .CustomValue(code =>
            {
                if (!code.HasValue())
                    return false;
                return code.Length > 2;
            }, "The product category code must be two digits at most.");
        }
    }
}
