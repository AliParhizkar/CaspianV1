using Caspian.Common;
using Employment.Model;
using FluentValidation;
using Caspian.Common.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Employment.Service
{
    public class OccupationService : SimpleService<Occupation>, ISimpleService<Occupation>
    {
        public OccupationService(IServiceScope scope)
            : base(scope)
        {
            RuleFor(t => t.Title).Required().UniqAsync("رسته شغلی با این عنوان در سیستم وجود دارد");
        }
    }
}
