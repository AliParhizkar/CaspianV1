using Caspian.Common;
using Investment.Model;
using Caspian.Common.Service;

namespace Investment.Service
{
    public class ExchangeService: BaseService<Exchange>
    {
        public ExchangeService(IServiceProvider provider)
            :base(provider)
        {
            RuleFor(t => t.Name).Required().UniqueAsync("ارزی با این عنوان در سیستم ثبت شده است");
        }
    }
}
