using Demo.Model;
using Caspian.Common;
using Caspian.Common.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Demo.Service
{
    public class ProvinceService : SimpleService<Province>, ISimpleService<Province>
    {
        public ProvinceService(IServiceScope scope)
            : base(scope)
        {
            RuleFor(t => t.Title).Required().UniqAsync("استانی با این عنوان تعریف شده است");
        }
    }
}
