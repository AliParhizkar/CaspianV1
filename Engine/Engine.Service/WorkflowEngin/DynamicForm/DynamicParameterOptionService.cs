using Caspian.Common;
using Caspian.Common.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Caspian.Engine.Service
{
    public class DynamicParameterOptionService : BaseService<DynamicParameterOption>
    {
        public DynamicParameterOptionService(IServiceProvider provider)
            :base(provider)
        {
            RuleFor(t => t.FaTitle).Required().UniqueAsync(t => t.DynamicParameterId, "عنوان فارسی نمی تواند تکراری باشد");
            RuleFor(t => t.EnTitle).Required().UniqueAsync(t => t.DynamicParameterId, "عنوان لاتین نمی تواند تکراری باشد")
                .CustomValue(t => t.IsValidIdentifire(), "برای عنوان لاتین فقط از حروف لاتین و اعداد می توانید استفاده نمایید.");
            RuleFor(t => t.Value).Required().UniqueAsync(t => t.DynamicParameterId, "مقدار نمی تواند تکراری باشد")
                .CustomValue(t => t <= 0, "مقدار باید بزرگتر از صفر باشد");
        }
    }
}
