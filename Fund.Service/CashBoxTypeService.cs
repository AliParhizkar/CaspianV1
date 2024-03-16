using Caspian.Common;
using Caspian.Common.Service;
using FluentValidation;
using Fund.Model;

namespace Fund.Service
{
    public class CashBoxTypeService : BaseService<CashBoxType>, IBaseService<CashBoxType>
    {
        public CashBoxTypeService(IServiceProvider provider)
            : base(provider)
        {
            RuleFor(t => t.Code).Required();
            RuleFor(t => t.Name).MaximumLength(50).Required();
            RuleFor(t => t.InternationalName).MaximumLength(50);
            RuleFor(t => t.InternationalName).MaximumLength(200);
        }
    }
}