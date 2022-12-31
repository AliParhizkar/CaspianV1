using System;
using Demo.Model;
using Caspian.Common;
using Caspian.Common.Service;

namespace Demo.Service
{
    public class MainUnitService : SimpleService<MainUnit>, ISimpleService<MainUnit>
    {
        public MainUnitService(IServiceProvider provider)
            : base(provider)
        {
            RuleFor(t => t.Title).Required().UniqAsync("منطقه ای با این عنوان در این شهر تعریف شده است.");
        }
    }
}
