using Demo.Model;
using Caspian.Common;
using Caspian.Common.Service;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Demo.Service
{
    public class StoreHouseService : SimpleService<StoreHouse>, ISimpleService<StoreHouse>
    {
        public StoreHouseService(IServiceProvider provider)
            : base(provider)
        {
            RuleFor(t => t.Title).Required().UniqAsync("انباری با این عنوان در سیستم تعریف شده است.");
        }
    }
}
