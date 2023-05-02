using System;
using Demo.Model;
using Caspian.Common;
using Caspian.Common.Service;

namespace Demo.Service
{
    public class CityService : BaseService<City>, IBaseService<City>
    {
        public CityService(IServiceProvider provider)
            : base(provider)
        {
            RuleFor(t => t.Title).Required().UniqAsync(t => t.CountryId, "A city with this title is defined in the system");
        }
    }
}
