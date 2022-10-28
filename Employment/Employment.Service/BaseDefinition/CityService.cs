using Caspian.Common;
using Employment.Model;
using Caspian.Common.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Employment.Service
{
    public class CityService : SimpleService<City>, ISimpleService<City>
    {
        public CityService(IServiceScope scope)
            : base(scope)
        {
            RuleFor(t => t.Title).Required().UniqAsync("شهری با این عنوان در سیستم ثبت شده است");
        }
    }
}
