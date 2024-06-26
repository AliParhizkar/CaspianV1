﻿using Caspian.Common;
using Caspian.Engine.Model;
using Caspian.Common.Service;
using Microsoft.EntityFrameworkCore;

namespace Caspian.Engine.Service
{
    public class MenuService : BaseService<Menu>, IBaseService<Menu>
    {
        public MenuService(IServiceProvider provider)
            :base(provider)
        {
            RuleFor(t => t.Title).Required(t => t.Id > 0).UniqAsync(t => t.SubSystemKind, "منویی با این عنوان در سیستم تعریف شده است");
            RuleFor(t => t.Source).Required().UniqAsync("صفحه ای با این آدرس در سیستم ثبت شده است");
            RuleFor(t => t.URL).Required(t => t.ShowonMenu && t.Id > 0);
            RuleFor(t => t.MenuCategoryId).Required(t => t.ShowonMenu && t.Id > 0)
                .Custom(t => t.MenuCategoryId.HasValue && !t.ShowonMenu, "در صورت عدم نمایش منو نیازی به گروه آن نیست");
            RuleFor(t => t.Ordering).Custom(t => t.Ordering < 0 || t.Id == 0 && t.Ordering == 0, "مقدار فیلد مرتب سازی باید بزرگتر  از صفر باشد").UniqAsync(t => t.MenuCategoryId, "فیلد مرتب سازی به ازای هر گروه منو نمی تواند تکراری باشد.");
        }

        public async override Task<Menu> AddAsync(Menu entity)
        {
            var ordering = (await GetAll().MaxAsync(t => (int?)t.Ordering)).GetValueOrDefault() + 1;
            entity.Ordering = ordering;
            return await base.AddAsync(entity);
        }
    }
}
