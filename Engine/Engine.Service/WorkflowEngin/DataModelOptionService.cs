
using Caspian.Common;
using Caspian.Common.Service;

namespace Caspian.Engine.Service
{
    public class DataModelOptionService : BaseService<DataModelOption>
    {
        public DataModelOptionService(IServiceProvider provider)
    : base(provider)
        {
            RuleFor(t => t.Title).Required().UniqueAsync("گزینه ای با این عنوان در سیستم ثبت شده است");
            RuleFor(t => t.Name).Required().UniqueAsync("گزینه ای با این نام در سیستم ثبت شده است")
                .Custom(t => t.Name.IsValidIdentifire(), "لطفا فقط از اعداد و حروف لاتین برای نامگذاری استفاده نمایید.");
        }
    }
}
