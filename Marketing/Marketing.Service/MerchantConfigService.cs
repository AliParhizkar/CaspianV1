using Caspian.Common;
using Marketing.Model;
using Caspian.Common.Service;

namespace Marketing.Service
{
    public class MerchantConfigService : BaseService<MerchantConfig>, IBaseService<MerchantConfig>
    {
        public MerchantConfigService(IServiceProvider provider)
            : base(provider)
        {
            RuleFor(t => t.Name).Required();
        }
        static MerchantConfig merchantConfig;

        public static MerchantConfig MerchantConfig
        {
            get
            {
                if (merchantConfig == null)
                {
                    using var context = new Context();
                    merchantConfig = context.MerchantConfigs.First();
                }
                return merchantConfig;
            }
        }
    }
}
