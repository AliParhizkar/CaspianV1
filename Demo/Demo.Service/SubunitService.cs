using System;
using Demo.Model;
using Caspian.Common;
using Caspian.Common.Service;

namespace Demo.Service
{
    public class SubunitService : SimpleService<Subunit>, ISimpleService<Subunit>
    {
        public SubunitService(IServiceProvider provider)
            : base(provider)
        {
            RuleFor(t => t.Title).Required().UniqAsync("واحد اصلی با این عنوان در سیستم تعریف شده است");
            RuleFor(t => t.Factor).CustomValue(t => t <= 0, "ضریب باید بزرگتر از صفر باشد");
        }
    }
}
