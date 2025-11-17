using Caspian.Common;
using Warehouse.Model;
using Caspian.Common.Service;
using Microsoft.EntityFrameworkCore;

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

        public override async Task<ProductCategory> AddAsync(ProductCategory category)
        {
            if (category.CategoryId.HasValue)
            {
                var parent = await SingleAsync(category.CategoryId.Value);
                if (!parent.HasChild)
                    parent.HasChild = true;
            }
            return await base.AddAsync(category);
        }

        public override async Task RemoveAsync(int id)
        {
            var old = await SingleAsync(id);
            if (old.CategoryId.HasValue)
            {
                var parent = await SingleAsync(old.CategoryId.Value);
                parent.HasChild = await GetAll().AnyAsync(t => t.CategoryId == parent.Id && t.Id != id);
            }
            await base.RemoveAsync(id);
        }
    }
}
