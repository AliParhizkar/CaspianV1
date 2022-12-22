using Caspian.Common;
using Employment.Model;
using Caspian.Common.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Employment.Service
{
    public class ExtraFactorService : SimpleService<ExtraFactor>, ISimpleService<ExtraFactor>
    {
        public ExtraFactorService(ServiceProvider provider)
            : base(provider)
        {
            RuleFor(t => t.BaseType).UniqAsync("این حالت قبلا ثبت شده است");
        }
    }
}
