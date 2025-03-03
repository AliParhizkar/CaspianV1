using System;
using Demo.Model;
using Caspian.Common;
using Caspian.Common.Service;

namespace Demo.Service
{
    public class CountryService : BaseService<Country>, IBaseService<Country>
    {
        public CountryService(IServiceProvider provider)
            : base(provider)
        {
            RuleFor(t => t.Title).Required().UniqueAsync("A country with this title is defined in the system");
            RuleFor(t => t.ActiveType).Custom(t => t.Id == 0 && t.ActiveType != ActiveType.Enable, "In Insert Country should be Active");
        }
    }

    public class ProvinceService : BaseService<Province>, IBaseService<Province>
    {
        public ProvinceService(IServiceProvider provider)
            : base(provider)
        {
            RuleFor(t => t.Title).Required().UniqueAsync("A country with this title is defined in the system");
        }
    }
}
