using System;
using Demo.Model;
using Caspian.Common;
using Caspian.Common.Service;

namespace Demo.Service
{
    public class CityService : BaseService<City>
    {
        public CityService(IServiceProvider provider)
            : base(provider)
        {
            RuleFor(t => t.Title).Required().UniqueAsync(t => t.CountryId, "A city with this title is defined in the system");
        }
    }

    public class TestService: BaseService<TestProduct>
    {
        public TestService(IServiceProvider provider)
            : base(provider)
        {
            
        }
    }
}
