using Caspian.Common;
using Employment.Model;
using Caspian.Common.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Employment.Service
{
    public class MajorService : BaseService<Major>, IBaseService<Major>
    {
        public MajorService(IServiceProvider provider)
            : base(provider)
        {
            RuleFor(t => t.Title).Required().UniqAsync("رشته تحصیلی با این عنوان در سیستم ثبت شده است");
        }
    }
}
