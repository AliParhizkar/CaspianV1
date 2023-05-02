using Caspian.Common;
using Employment.Model;
using Caspian.Common.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Employment.Service
{
    public class ProvinceService : BaseService<Province>, IBaseService<Province>
    {
        public ProvinceService(IServiceProvider provider)
            : base(provider)
        {
            RuleFor(t => t.Title).Required().UniqAsync("استانی با این عنوان در سیستم ثبت شده است");
        }
    }
}
