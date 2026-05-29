using Caspian.Common;
using Caspian.Engine.Model;
using Caspian.Common.Service;
using Microsoft.EntityFrameworkCore;

namespace Caspian.Engine.Service
{
    public class MenuService : BaseService<Menu>
    {
        public MenuService(IServiceProvider provider)
            :base(provider)
        {
            RuleFor(t => t.Title).Required(t => t.Id > 0)
                .CustomAsync(async t =>
                {
                    if (await GetAll().AnyAsync(u => t.Id != u.Id && t.Title == u.Title && t.SubsystemKind == u.SubsystemKind))
                        return $"منویی با عنوان {t.Title} در این سامانه تعریف شده است";
                    return null;
                });
            RuleFor(t => t.SourceId).Required().UniqueAsync(t => t.SubsystemKind, "آدرسی با این شماره در سیستم ثبت شده است");
            RuleFor(t => t.URL).Required(t => t.ShowOnMenu && t.Id > 0);
            RuleFor(t => t.MenuCategoryId).Required(t => t.ShowOnMenu && t.Id > 0)
                .Custom(t => t.MenuCategoryId.HasValue && !t.ShowOnMenu, "در صورت عدم نمایش منو نیازی به گروه آن نیست");
            RuleFor(t => t.Ordering).Custom(t => t.Ordering < 0 || t.Id == 0 && t.Ordering == 0, "مقدار فیلد مرتب سازی باید بزرگتر  از صفر باشد")
                .UniqueAsync(t => t.MenuCategoryId, "فیلد مرتب سازی به ازای هر گروه منو نمی تواند تکراری باشد.");
        }

        public async override Task<Menu> AddAsync(Menu entity)
        {
            var ordering = (await GetAll().MaxAsync(t => (int?)t.Ordering)).GetValueOrDefault() + 1;
            entity.Ordering = ordering;
            return await base.AddAsync(entity);
        }

        public async Task IncOrderingAsync(int id)
        {
            var old = await SingleAsync(id);
            var pre = await GetAll().Where(t => t.MenuCategoryId == old.MenuCategoryId && t.Ordering < old.Ordering)
                .OrderByDescending(t => t.Ordering).FirstOrDefaultAsync();
            if (pre != null)
            {
                var preOrdering = pre.Ordering;
                var oldOrdering = old.Ordering;
                //For Unique ordering set ordering to unique values
                old.Ordering = int.MaxValue;
                pre.Ordering = int.MaxValue - 1;
                await base.UpdateAsync(old);
                await base.UpdateAsync(pre);
                await SaveChangesAsync();
                ///
                pre.Ordering = oldOrdering;
                old.Ordering = preOrdering;
                await base.UpdateAsync(pre);
                await base.UpdateAsync(old);
                await SaveChangesAsync();
            }
        }

        public async Task DecOrderingAsync(int id)
        {
            var old = await SingleAsync(id);
            var next = await GetAll().Where(t => t.MenuCategoryId == old.MenuCategoryId && t.Ordering > old.Ordering).OrderBy(t => t.Ordering).FirstOrDefaultAsync();
            if (next != null)
            {
                var oldOrdering = old.Ordering;
                var nexOrdering = next.Ordering;
                //For Unique ordering set ordering to unique values
                old.Ordering = int.MaxValue;
                next.Ordering = int.MaxValue - 1;
                await base.UpdateAsync(next);
                await base.UpdateAsync(old);
                await SaveChangesAsync();
                old.Ordering = nexOrdering;
                next.Ordering = oldOrdering;
                await base.UpdateAsync(next);
                await base.UpdateAsync(old);
                await SaveChangesAsync();
            }
        }
    }
}
