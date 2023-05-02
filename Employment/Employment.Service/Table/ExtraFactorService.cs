using Caspian.Common;
using Employment.Model;
using Caspian.Common.Service;

namespace Employment.Service
{
    public class ExtraFactorService : BaseService<ExtraFactor>, IBaseService<ExtraFactor>
    {
        public ExtraFactorService(IServiceProvider provider)
            : base(provider)
        {
            RuleFor(t => t.BaseType).UniqAsync("این حالت قبلا ثبت شده است");
        }
    }
}
