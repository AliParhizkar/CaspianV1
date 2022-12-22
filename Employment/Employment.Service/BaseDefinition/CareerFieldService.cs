using Caspian.Common;
using Employment.Model;
using Caspian.Common.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Employment.Service
{
    public class CareerFieldService : SimpleService<CareerField>, ISimpleService<CareerField>
    {
        public CareerFieldService(IServiceProvider provider)
            : base(provider)
        {
            RuleFor(t => t.Title).Required().UniqAsync("رشته شغلی با این عنوان در سیستم وجود دارد");
        }
    }
}
