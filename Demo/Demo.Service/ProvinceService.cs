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

        public override Task<Country> AddAsync(Country entity)
        {
            /// برنامه نویس می تواند مقدار یک Property را پر کند 
            /// می تواند یک Entity دیگر را بروزرسانی کند
            /// می تواند یک Entity دیگر هم ثبت کند
            return base.AddAsync(entity);
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
