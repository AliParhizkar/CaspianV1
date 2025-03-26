using Caspian.Common;
using Marketing.Model;
using Caspian.Common.Service;
using Microsoft.EntityFrameworkCore;

namespace Marketing.Service
{
    public class ConfigService : BaseService<Config>, IBaseService<Config>
    {
        public ConfigService(IServiceProvider provider)
            : base(provider)
        {
            RuleFor(t => t.Name).Required();
        }
        static Config merchantConfig;

        public static Config Config
        {
            get
            {
                if (merchantConfig == null)
                {
                    using var context = new Context();
                    merchantConfig = context.MerchantConfigs.AsNoTracking().SingleOrDefault();
                }
                return merchantConfig;
            }
            set
            {
                merchantConfig = value;
            }
        }
    }
}
