using Caspian.Common;
using Employment.Model;
using Caspian.Common.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Employment.Service
{
    public class OccupationService : SimpleService<Occupation>, ISimpleService<Occupation>
    {
        public OccupationService(ServiceProvider provider)
            : base(provider)
        {
            RuleFor(t => t.Title).Required().UniqAsync("رسته شغلی با این عنوان در سیستم وجود دارد");
        }
    }
}
