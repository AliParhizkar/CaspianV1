using System;
using Demo.Model;
using Caspian.Common;
using Caspian.Common.Service;

namespace Demo.Service
{
    public class MainUnitService : BaseService<MainUnit>, IBaseService<MainUnit>
    {
        public MainUnitService(IServiceProvider provider)
            : base(provider)
        {
            RuleFor(t => t.Title).Required().UniqAsync("Mainunit title should uniq");
        }
    }
}
