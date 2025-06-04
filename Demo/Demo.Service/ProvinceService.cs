using System;
using Demo.Model;
using Caspian.Common;
using Caspian.Common.Service;
using System.Threading.Tasks;

namespace Demo.Service
{
    public class CountryService : BaseService<Country>
    {
        public CountryService(IServiceProvider provider)
            : base(provider)
        {
            RuleFor(t => t.Title).Required().UniqueAsync("A country with this title is defined in the system");
        }
    }

    public class ProvinceService : BaseService<Province>
    {
        public ProvinceService(IServiceProvider provider)
            : base(provider)
        {
            RuleFor(t => t.Title).Required().UniqueAsync("A country with this title is defined in the system");
        }
    }
}
