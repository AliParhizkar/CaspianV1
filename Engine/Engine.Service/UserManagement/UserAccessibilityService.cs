using Caspian.Common;
using Caspian.Engine.Model;
using Caspian.Common.Service;
using Microsoft.EntityFrameworkCore;

namespace Caspian.Engine.Service
{
    public class MenuAccessibilityService : BaseService<MenuAccessibility>
    {
        public MenuAccessibilityService(IServiceProvider provider) :
            base(provider)
        {
            RuleFor(t => t.UserId).Custom(t => t.RoleId == null && t.UserId == null, "یکی از فیلدهای کاربر و یا نقش باید مشخص باشند")
                .Custom(t => t.RoleId.HasValue && t.UserId.HasValue, "فقط یکی از فیلدهای کاربر و یا نقش بای دمشخص باشند");

            RuleFor(t => t.RoleId).Custom(t => t.RoleId == null && t.UserId == null, "یکی از فیلدهای کاربر و یا نقش باید مشخص باشند")
                .Custom(t => t.RoleId.HasValue && t.UserId.HasValue, "فقط یکی از فیلدهای کاربر و یا نقش بای دمشخص باشند");
        }

        /// <summary>
        /// کد تمامی منوهایی که کاربر به آنها دسترسی دارد را برمی گرداند
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<IList<int>> GetUserMenus(int userId)
        {
            if (userId == 1)
            {
                return await new MenuService(ServiceProvider).GetAll().Where(t => t.ShowonMenu)
                    .Select(t => t.Id).ToListAsync();
            }
            var query = GetAll().Where(t => t.UserId == userId || t.Role.Memberships.Any(u => u.UserId == userId))
                .Select(t => t.MenuId).Distinct();
            return await query.ToListAsync();
        }
    }
}
