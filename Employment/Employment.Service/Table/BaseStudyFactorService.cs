using Caspian.Common;
using Employment.Model;
using Caspian.Common.Service;

namespace Employment.Service
{
    public class BaseStudyFactorService : BaseService<BaseStudyFactor>, IBaseService<BaseStudyFactor>
    {
        public BaseStudyFactorService(IServiceProvider provider)
            : base(provider)
        {
            RuleFor(t => t.BaseStudy).CustomValue(t => t < BaseStudy.AssociateDegree, "برای مقاطع کمتر از فوق دیپلم ضریب درک تحصیلی تعریف نشده است")
                .UniqAsync("این حالت قبلا ثبت شده است");
        }
    }
}
