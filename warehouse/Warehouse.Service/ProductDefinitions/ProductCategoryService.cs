using Caspian.Common;
using Warehouse.Model;
using Caspian.Common.Service;

namespace Warehouse.Service
{
    public class ProductCategoryService: BaseService<ProductCategory>
    {
        public ProductCategoryService(IServiceProvider provider):
            base(provider)
        {
            RuleFor(t => t.Title).Required().UniqueAsync("گروه محصولی با این عنوان در سیستم ثبت شده است");
            RuleFor(t => t.Code).Required().UniqueAsync("گروه محصولی با این کد در سیستم ثبت شده است")
                .CustomAsync(async t => 
                {
                    if (t.CategoryId == null)
                        return null;
                    var parent = await SingleAsync(t.CategoryId.Value);
                    if (t.Code.StartsWith(parent.Code))
                        return null;
                    return $"کد باید با {parent.Code} شروع شود";
                });
        }
    }
}
