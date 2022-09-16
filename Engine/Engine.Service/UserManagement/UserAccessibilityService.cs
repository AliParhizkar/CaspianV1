using Caspian.Common;
using Caspian.Engine.Model;
using Caspian.Common.Service;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace Caspian.Engine.Service
{
    public class UserAccessibilityService : SimpleService<UserAccessibility>
    {
        public UserAccessibilityService(IServiceScope scope):
            base(scope)
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
            var query = GetAll().Where(t => t.UserId == userId || t.Role.Memberships.Any(u => u.UserId == userId))
                .Select(t => t.MenuId).Distinct();
            return await query.ToListAsync();
        }
    }
}
