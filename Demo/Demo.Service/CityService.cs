using System;
using Demo.Model;
using Caspian.Common;
using Caspian.Common.Service;

namespace Demo.Service
{
    public class CityService : SimpleService<City>, ISimpleService<City>
    {
        public CityService(IServiceProvider provider)
            : base(provider)
        {
            RuleFor(t => t.Title).Required().UniqAsync(t => t.CountryId, "a city with this title is defined in the system");
        }
    }
}
