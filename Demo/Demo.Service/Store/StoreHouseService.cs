using Demo.Model;
using Caspian.Common;
using Caspian.Common.Service;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Demo.Service
{
    public class WareHouseService : SimpleService<Warehouse>, ISimpleService<Warehouse>
    {
        public WareHouseService(IServiceProvider provider)
            : base(provider)
        {
            RuleFor(t => t.Title).Required().UniqAsync("انباری با این عنوان در سیستم تعریف شده است.");
        }
    }
}
