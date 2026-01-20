using Investment.Model;
using Caspian.Common.Service;
using Caspian.Common;
using Microsoft.EntityFrameworkCore;

namespace Investment.Service
{
    public class ServiceGroupService: BaseService<ServiceGroup>
    {
        public ServiceGroupService(IServiceProvider provider)
            :base(provider)
        {
            RuleFor(t => t.Name).Required().UniqueAsync("گروه خدماتی با این نام در سیستم تعریغف شده است");
            RuleFor(t => t.Code).Required().UniqueAsync("گروه خدماتی با این کد در سیستم تعریف شده است")
                .CustomAsync(async t =>
                {
                    var length = await provider.GetCaspianService<ServiceGroupLevelService>().GetAll().Where(u => u.LevelType <= t.ServiceGroupLevelType).SumAsync(t => t.Length);
                    if (length != t.Code.Length)
                        return $"طول کد باید {length} باشد.";
                    return null;
                }).CustomAsync(async t =>
                {
                    if (t.ParentId.HasValue)
                    {
                        var parent = await SingleAsync(t.ParentId.Value);
                        if (!t.Code.StartsWith(parent.Code))
                            return $"کد باید با {parent.Code} شروع شود";
                    }
                    return null;
                });
            RuleFor(t => t.ServiceGroupLevelType).CustomAsync(async t =>
            {
                if (t.ParentId.HasValue)
                {
                    var parent = await SingleAsync(t.ParentId.Value);
                    if (t.ServiceGroupLevelType != parent.ServiceGroupLevelType + 1)
                        return $"سطح کد باید {(parent.ServiceGroupLevelType + 1).EnumText()} باشد";
                }
                else if (t.ServiceGroupLevelType != ServiceGroupLevelType.Level1)
                    return $"سطح کد باید {ServiceGroupLevelType.Level1.EnumText()}";
                return null;
            });
        }
    }
}
