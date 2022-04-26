using Caspian.Common;
using Caspian.Common.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Caspian.Engine.Service
{
    public class ReportGroupService : SimpleService<ReportGroup>, ISimpleService<ReportGroup>
    {
        public ReportGroupService(IServiceScope scope)
            :base(scope)
        {
            RuleFor(t => t.Title).Required().UniqAsync("گزارشی با این عنوان در سیستم ثبت شده است");
            RuleFor(t => t.NameSpace).Required();
            RuleFor(t => t.ClassTitle).Required();
            RuleFor(t => t.MethodName).Required();
        }

        async public override Task UpdateAsync(ReportGroup entity)
        {
            var old = await SingleAsync(entity.Id);
            old.Disable = entity.Disable;
            old.Descript = entity.Descript;
            await base.UpdateAsync(old);
        }

        public override void Remove(ReportGroup entity)
        {
            throw new Exception("امکان حذف گروه گزارش وجود ندارد.");
        }
    }
}
