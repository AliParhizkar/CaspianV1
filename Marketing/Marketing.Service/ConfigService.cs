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
            RuleFor(t => t.RoundAmount).CustomValue(t => t == 0, "مبلغ رند نمی تواند صفر باشد");
            RuleFor(t => t.PercentDiscount).CustomValue(t => t < 0 || t > 100, "تخفیف باید بین صفر تا صد باشد");
        }
        static Config merchantConfig;

        public static Config Config
        {
            get
            {
                if (merchantConfig == null)
                {
                    using var context = new MarketingContext();
                    merchantConfig = context.Set<Config>().AsNoTracking().SingleOrDefault();
                }
                return merchantConfig;
            }
            set
            {
                merchantConfig = value;
            }
        }

        public static decimal GetRoundValue(decimal amount)
        {
            if (Config.RoundAmount == null)
                return 0;
            var roundValue = amount % Config.RoundAmount.Value;
            if (roundValue == Config.RoundAmount.Value || roundValue == 0)
                return 0;
            switch (Config.RoundType)
            {
                case RoundType.ToDown:
                    return -roundValue;
                case RoundType.ToUp:
                    return Config.RoundAmount.Value - roundValue;
                case RoundType.Standard:
                    if (roundValue * 2 > Config.RoundAmount.Value)
                        return Config.RoundAmount.Value - roundValue;
                    return -roundValue;
            }
            throw new NotImplementedException();
        }
    }
}
