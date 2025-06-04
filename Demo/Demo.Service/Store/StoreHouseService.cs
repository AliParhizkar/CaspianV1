using System;
using Demo.Model;
using Caspian.Common;
using Caspian.Common.Service;

namespace Demo.Service
{
    public class WareHouseService : BaseService<Warehouse>
    {
        public WareHouseService(IServiceProvider provider)
            : base(provider)
        {
            RuleFor(t => t.Title).Required().UniqueAsync("A warehouse with this title is defined in the system.");
        }
    }
}
