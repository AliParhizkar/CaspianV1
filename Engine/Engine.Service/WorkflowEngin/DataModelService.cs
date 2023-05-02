using Caspian.Common;
using Caspian.Common.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Caspian.Engine.Service
{
    public class DataModelService : BaseService<DataModel>
    {
        public DataModelService(IServiceProvider provider)
            : base(provider)
        {
            RuleFor(t => t.Title).Required().UniqAsync("مدل داده ای با این عنوان در سیستم وجود دارد");
            RuleFor(t => t.Name).Required().UniqAsync("مدل داده ای با این نام در سیستم وجود دارد")
                .CustomValue(t => t.IsValidIdentifire(), "برای نام فقط از کارکترهای لاتین و اعداد استفاده کنید");
        }
    }

}
