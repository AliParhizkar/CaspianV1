using Caspian.Common;
using Warehouse.Model;
using Caspian.Common.Service;
using Microsoft.EntityFrameworkCore;

namespace Warehouse.Service
{
    //public class ProductCategoryService: BaseService<ProductCategory>
    //{
    //    public static readonly int[] LevelsLength = { 1, 2, 3, 3, 3 };
    //    public ProductCategoryService(IServiceProvider provider):
    //        base(provider)
    //    {
    //        RuleFor(t => t.Title).Required().UniqueAsync("گروه محصولی با این عنوان در سیستم ثبت شده است");
    //        RuleFor(t => t.Code).Required().UniqueAsync("گروه محصولی با این کد در سیستم ثبت شده است")
    //            .CustomAsync(async t => 
    //            {
    //                if (t.CategoryId == null)
    //                {
    //                    if (t.Code.Length != LevelsLength[0])
    //                        return $"طول کد باید {LevelsLength[0]} باشد";
    //                }
    //                else
    //                {
    //                    var parent = await SingleAsync(t.CategoryId.Value);
    //                    if (parent.Level == LevelsLength.Length)
    //                        return $"امکان تعریف بیش از{LevelsLength.Length} سطح کد وجود ندارد";
    //                    var sum = LevelsLength.Where((t, index) => index <= parent.Level).Sum(t => t);
    //                    if (t.Code.Length != sum)
    //                        return $"طول کد باید {sum} رقم باشد";
    //                    if (!t.Code.StartsWith(parent.Code))
    //                        return $"کد باید با {parent.Code} شروع شود";
    //                }
    //                return null;
    //            });
    //    }

    //    public override async Task<ProductCategory> AddAsync(ProductCategory category)
    //    {
    //        if (category.CategoryId.HasValue)
    //        {
    //            var parent = await SingleAsync(category.CategoryId.Value);
    //            if (!parent.HasChild)
    //                parent.HasChild = true;
    //            category.Level = parent.Level + 1;
    //        }
    //        else
    //            category.Level = 1;
    //        return await base.AddAsync(category);
    //    }

    //    public override async Task UpdateAsync(ProductCategory category)
    //    {
    //        var old = await SingleAsync(category.Id);
    //        category.Level = old.Level;
    //        await base.UpdateAsync(category);
    //    }

    //    public override async Task RemoveAsync(int id)
    //    {
    //        var old = await SingleAsync(id);
    //        if (old.CategoryId.HasValue)
    //        {
    //            var parent = await SingleAsync(old.CategoryId.Value);
    //            parent.HasChild = await GetAll().AnyAsync(t => t.CategoryId == parent.Id && t.Id != id);
    //        }
    //        await base.RemoveAsync(id);
    //    }
    //}
}
