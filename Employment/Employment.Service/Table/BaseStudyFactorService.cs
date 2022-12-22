using Caspian.Common;
using Employment.Model;
using Caspian.Common.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Employment.Service
{
    public class BaseStudyFactorService : SimpleService<BaseStudyFactor>, ISimpleService<BaseStudyFactor>
    {
        public BaseStudyFactorService(ServiceProvider provider)
            : base(provider)
        {
            RuleFor(t => t.BaseStudy).CustomValue(t => t < BaseStudy.AssociateDegree, "برای مقاطع کمتر از فوق دیپلم ضریب درک تحصیلی تعریف نشده است")
                .UniqAsync("این حالت قبلا ثبت شده است");
        }
    }
}
