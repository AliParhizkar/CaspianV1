using Caspian.Common;
using Caspian.Engine.Model;
using Caspian.Common.Service;
using Microsoft.EntityFrameworkCore;

namespace Caspian.Engine.Service
{
    public class MenuCategoryService : BaseService<MenuCategory>
    {
        public MenuCategoryService(IServiceProvider provider)
            :base(provider)
        {
            RuleFor(t => t.Title).Required().UniqueAsync(t => t.SubsystemKind, "گروه منویی با این عنوان در سیستم ثبت شده است");
            RuleFor(t => t.IconFont).Required().UniqueAsync(t => t.SubsystemKind, "گروه منویی با این آیکون در سیستم تعریف شده است");
        }

        public async override Task<MenuCategory> AddAsync(MenuCategory entity)
        {
            var ordering = (await GetAll().Where(t => t.SubsystemKind == entity.SubsystemKind)
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


        public async Task IncOrderingAsync(int id)
        {
            var old = await SingleAsync(id);
            var pre = await GetAll().Where(t => t.SubsystemKind == old.SubsystemKind && t.Ordering < old.Ordering)
                .OrderByDescending(t => t.Ordering).FirstOrDefaultAsync();
            if (pre != null)
            {
                var temp = old.Ordering;
                old.Ordering = pre.Ordering;
                pre.Ordering = temp;
                await base.UpdateAsync(pre);
                await base.UpdateAsync(old);
            }
        }

        public async Task DecOrderingAsync(int id)
        {
            var old = await SingleAsync(id);
            var next = await GetAll().Where(t => t.SubsystemKind == old.SubsystemKind && t.Ordering > old.Ordering).OrderBy(t => t.Ordering).FirstOrDefaultAsync();
            if (next != null)
            {
                var temp = old.Ordering;
                old.Ordering = next.Ordering;
                next.Ordering = temp;
                await base.UpdateAsync(next);
                await base.UpdateAsync(old);
            }
        }
    }
}
