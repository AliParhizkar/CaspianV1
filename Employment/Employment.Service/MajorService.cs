using Caspian.Common;
using Employment.Model;
using Caspian.Common.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Employment.Service
{
    public class MajorService : SimpleService<Major>, ISimpleService<Major>
    {
        public MajorService(IServiceScope scope)
            : base(scope)
        {
            RuleFor(t => t.Title).Required().UniqAsync("رشته تحصیلی با این عنوان در سیستم ثبت شده است");
        }
    }
}
