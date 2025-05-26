using Caspian.Common;
using Caspian.Engine.Model;
using Caspian.Common.Service;

namespace Caspian.Engine.Service
{
    public class ReportGroupService : MasterDetailsService<ReportGroup, ReportGroupParameter>, 
        IMasterDetailsService<ReportGroup, ReportGroupParameter>
    {
        public ReportGroupService(IServiceProvider provider)
            :base(provider)
        {
            RuleFor(t => t.Title).Required().UniqueAsync("گزارشی با این عنوان در سیستم ثبت شده است");
            RuleFor(t => t.NameSpace).Required();
            RuleFor(t => t.ClassTitle).Required();
            RuleFor(t => t.MethodName).Required();

            RuleForEach(t => t.ReportGroupParameters).SetValidator(t => new ReportGroupParameterService(provider, t.ReportGroupParameters));
        }

        async public override Task UpdateAsync(ReportGroup entity)
        {
            var old = await SingleAsync(entity.Id);
            old.Disable = entity.Disable;
            old.Descript = entity.Descript;
            await base.UpdateAsync(old);
        }

        public override Task RemoveAsync(int id)
        {
            throw new Exception("امکان حذف گروه گزارش وجود ندارد.");
        }
    }
}
