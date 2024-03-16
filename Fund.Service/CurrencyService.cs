using Caspian.Common;
using Caspian.Common.Service;
using FluentValidation;
using Fund.Model;

namespace Fund.Service
{
    public class CurrencyService : BaseService<Currency>, IBaseService<Currency>
    {
        public CurrencyService(IServiceProvider provider)
            : base(provider)
        {
            RuleFor(t => t.Code).MaximumLength(50).Required();
            RuleFor(t => t.Name).MaximumLength(50).Required();
        }
    }
}