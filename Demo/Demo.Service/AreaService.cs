using Demo.Model;
using Caspian.Common;
using Caspian.Common.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Demo.Service
{
    public class AreaService : SimpleService<Area>, ISimpleService<Area>
    {
        public AreaService(IServiceScope scope)
            : base(scope)
        {
            RuleFor(t => t.Title).Required().UniqAsync(t => t.CityId, "منطقه ای با این عنوان در این شهر تعریف شده است.");
        }
    }
}
