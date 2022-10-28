using Caspian.Common;
using Employment.Model;
using Caspian.Common.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Employment.Service
{
    public class ProvinceService : SimpleService<Province>, ISimpleService<Province>
    {
        public ProvinceService(IServiceScope scope)
            : base(scope)
        {
            RuleFor(t => t.Title).Required().UniqAsync("استانی با این عنوان در سیستم ثبت شده است");
        }
    }
}
