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
            RuleFor(t => t.Title).Required().UniqAsync("A country with this title is defined in the system");
        }
    }
}
