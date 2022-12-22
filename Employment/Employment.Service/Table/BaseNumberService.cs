using Caspian.Common;
using Employment.Model;
using Caspian.Common.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Employment.Service
{
    public class BaseNumberService : SimpleService<BaseNumber>, ISimpleService<BaseNumber>
    {
        public BaseNumberService(IServiceProvider provider)
            : base(provider)
        {
            RuleFor(t => t.BaseStudy).CustomValue(t => t < BaseStudy.AssociateDegree, "عدد مبناء برای کمتر از کاردانی تعریف نشده است")
                .UniqAsync(t => t.BaseType, "این حالت قبلا ثبت شده است");
            RuleFor(t => t.BaseType).Custom(t => t.BaseStudy == BaseStudy.AssociateDegree && t.BaseType > BaseType.Rank3,
                "رتبه 2 و 1 برای مقطع کاردانی مجاز نمی باشد")
                .Custom(t => t.BaseStudy == BaseStudy.Bachelor && t.BaseType == BaseType.Rank1,
                "رتبه 1 برای مقطع کارشناسی مجاز نمی باشد");

        }
    }
}
