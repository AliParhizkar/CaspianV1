using System;
using System.Linq;
using System.Threading.Tasks;
using Caspian.Common.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Caspian.Common.Navigation
{
    public class MenuCategoryService : SimpleService<MenuCategory>, ISimpleService<MenuCategory>
    {
        public MenuCategoryService(IServiceScope scope)
            :base(scope)
        {
             RuleFor(t => t.Title).Required();
        }

        public async override Task<MenuCategory> AddAsync(MenuCategory entity)
        {
            var ordering = (await GetAll().Where(t => t.SubSystemKind == entity.SubSystemKind)
                .MaxAsync(t => (int?)t.Ordering)).GetValueOrDefault() + 1;
            entity.Ordering = ordering;
            return await base.AddAsync(entity);
        }

        public async override Task UpdateAsync(MenuCategory entity)
        {
            var old = await SingleAsync(entity.Id);
            entity.Ordering = old.Ordering;
            await base.UpdateAsync(entity);
        }

        public async Task IncPriority(MenuCategory curent, MenuCategory next)
        {
            var temp = curent.Ordering;
            curent.Ordering = next.Ordering;
            next.Ordering = temp;
            await base.UpdateAsync(curent);
            await base.UpdateAsync(next);
        }

        public async Task DecPriority(MenuCategory curent, MenuCategory pre)
        {
            var temp = curent.Ordering;
            curent.Ordering = pre.Ordering;
            pre.Ordering = temp;
            await base.UpdateAsync(curent);
            await base.UpdateAsync(pre);
        }
    }
}
