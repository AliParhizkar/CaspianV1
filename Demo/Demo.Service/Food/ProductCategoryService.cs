using System;
using Demo.Model;
using System.Linq;
using Caspian.Common;
using System.Threading.Tasks;
using Caspian.Common.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Demo.Service
{
    public class ProductCategoryService : SimpleService<ProductCategory>, ISimpleService<ProductCategory>
    {
        public ProductCategoryService(IServiceScope scope)
            : base(scope)
        {
            RuleFor(t => t.Title).Required().UniqAsync("گروه محصولی با این عنوان تعریف شده است");
            RuleFor(t => t.Code).UniqAsync("گروه محصولی با این کد تعریف شده است").Custom(pc => 
            {
                if (!pc.Code.HasValue())
                    return false;
                return !new ProductService(ServiceScope).GetAll().Any(p => p.Code == pc.Code);
            }, "محصولی با این کد تعریف شده است");
        }

        public async Task IncPriorityAsync(ProductCategory productCategory)
        {
            var pre = await GetAll().Where(t => t.Priority < productCategory.Priority).OrderByDescending(t => t.Priority).FirstOrDefaultAsync();
            if (pre != null)
            {
                var old = await SingleAsync(productCategory.Id);
                var temp = old.Priority;
                old.Priority = pre.Priority;
                pre.Priority = temp;
                await base.UpdateAsync(pre);
                await base.UpdateAsync(old);
            }
            
        }

        public async Task DecPriorityAsync(ProductCategory productCategory)
        {
            var next = await GetAll().Where(t => t.Priority > productCategory.Priority).OrderBy(t => t.Priority).FirstOrDefaultAsync();
            if (next != null)
            {
                var old = await SingleAsync(productCategory.Id);
                var temp = old.Priority;
                old.Priority = next.Priority;
                next.Priority = temp;
                await base.UpdateAsync(next);
                await base.UpdateAsync(old);
            }
        }

        public async override Task<ProductCategory> AddAsync(ProductCategory entity)
        {
            entity.Priority = await GetAll().MaxAsync(t => t.Priority) + 1;
            return  await base.AddAsync(entity);
        }

        public async override Task UpdateAsync(ProductCategory entity)
        {
            var old = await SingleAsync(entity.Id);
            var q = Context.Entry(old).State;
            entity.Priority = old.Priority;
            var q1 = Context.Entry(old).State;
            await base.UpdateAsync(entity);
        }
    }
}
