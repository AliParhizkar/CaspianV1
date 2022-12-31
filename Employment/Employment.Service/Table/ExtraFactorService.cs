using Caspian.Common;
using Employment.Model;
using Caspian.Common.Service;

namespace Employment.Service
{
    public class ExtraFactorService : SimpleService<ExtraFactor>, ISimpleService<ExtraFactor>
    {
        public ExtraFactorService(IServiceProvider provider)
            : base(provider)
        {
            RuleFor(t => t.BaseType).UniqAsync("این حالت قبلا ثبت شده است");
        }
    }
}
