using Caspian.Common;
using Investment.Model;
using Caspian.Common.Service;
using Microsoft.EntityFrameworkCore;

namespace Investment.Service
{
    public class ServiceGroupLevelService : BaseService<ServiceGroupLevel>
    {
        public ServiceGroupLevelService(IServiceProvider provider):
            base(provider)
        {
            RuleFor(t => t.Name).Required().UniqueAsync("سطحی با این عنوان در سیستم تعریف شده است");
            RuleFor(t => t.Length).Custom(t => t.Length <= 0, "طول سطح باید بزرگتر از صفر باشد").CustomAsync(async t =>
            {
                if (t.Id > 0)
                {
                    var old = await SingleAsync(t.Id);
                    if (old.Length != t.Length && await provider.GetCaspianService<ServiceGroupService>().GetAll()
                    .AnyAsync(u => u.ServiceGroupLevelType == t.LevelType))
                        return "این سطح در تعریف کدینگ خدمات استفاده شده است و امکان تغییر طول آن وجود ندارد";
                }
                return null;
            });
            RuleFor(t => t.LevelType).UniqueAsync("برای این سطح قبلا عنوان تعریف شده است").Custom(t =>
            {
                if (t.LevelType == ServiceGroupLevelType.Level1)
                    return false;
                var max = GetAll().Max(t => (int?)t.LevelType).GetValueOrDefault() + 1;
                return max != t.LevelType.ConvertToInt();
            }, "لطفا سطوح کدینگ را بترتیب وارد نمائید").CustomAsync(async t =>
            {
                if (t.Id > 0)
                {
                    var old = await SingleAsync(t.Id);
                    if (old.LevelType != t.LevelType && await provider.GetCaspianService<ServiceGroupService>().GetAll()
                    .AnyAsync(u => u.ServiceGroupLevelType == t.LevelType))
                        return "این سطح در تعریف کدینگ خدمات استفاده شده است و امکان تغییر نوع سطح آن وجود ندارد";
                }
                return null;
            });
            RuleForRemove().CustomAsync(async t => await provider.GetCaspianService<ServiceGroupService>().GetAll()
                    .AnyAsync(u => u.ServiceGroupLevelType == t.LevelType), 
                    "سطح کدینگ در تعریف کدینگ خدمات استفاده شده اند، بنابراین امکان حذف وجود ندارد");
        }
    }
}
