using Investment.Model;
using Caspian.Common.Service;
using Caspian.Common;

namespace Investment.Service
{
    public class DepreciationLawGroupService : BaseService<DepreciationLawGroup>
    {
        public DepreciationLawGroupService(IServiceProvider provider)
            :base(provider)
        {
            RuleFor(t => t.Name).Required().UniqueAsync("گروه قانون استهلاکی با این عنوان در سیستم ثبت شده است");
        }
    }
}
