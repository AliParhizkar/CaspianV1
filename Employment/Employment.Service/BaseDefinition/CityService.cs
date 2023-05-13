using Caspian.Common;
using Employment.Model;
using Caspian.Common.Service;

namespace Employment.Service
{
    public class CityService : BaseService<City>, IBaseService<City>
    {
        public CityService(IServiceProvider provider)
            : base(provider)
        {
            RuleFor(t => t.Title).Required().UniqAsync("A city with this title is registered in the system");
        }
    }
}
