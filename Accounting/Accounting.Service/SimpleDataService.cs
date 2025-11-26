using Accounting.Model;
using Caspian.Common;
using Caspian.Common.Service;

namespace Accounting.Service
{
    public class SimpleDataService : BaseService<SimpleData>
    {
        public SimpleDataService(IServiceProvider provider) : base(provider)
        {
            RuleFor(t => t.Title).Required().UniqueAsync("نوعی با این عنوان در سیستم حسابداری ثبت شده است");
        }
    }
}
