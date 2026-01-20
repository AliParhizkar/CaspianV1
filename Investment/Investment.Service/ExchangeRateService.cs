using Caspian.Common;
using Investment.Model;
using Caspian.Common.Service;

namespace Investment.Service
{
    public class ExchangeRateService: BaseService<ExchangeRate>
    {
        public ExchangeRateService(IServiceProvider provider)
            :base(provider)
        {
            RuleFor(t => t.Date).UniqueAsync(t => t.ExchangeId, "برای این ارز در این تاریخ نرخ تعریف شده است");
            RuleFor(t => t.Rate).CustomValue(t => t <= 0, "نرخ ارز باید عدد بزرگتر از صفر باشد");
        }
    }
}
