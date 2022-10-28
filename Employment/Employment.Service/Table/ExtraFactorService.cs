using Caspian.Common;
using Employment.Model;
using Caspian.Common.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Employment.Service
{
    public class ExtraFactorService : SimpleService<ExtraFactor>, ISimpleService<ExtraFactor>
    {
        public ExtraFactorService(IServiceScope scope)
            : base(scope)
        {
            RuleFor(t => t.BaseType).UniqAsync("این حالت قبلا ثبت شده است");
        }
    }
}
