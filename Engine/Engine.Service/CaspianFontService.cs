using Caspian.Common;
using Caspian.Engine.Model;
using Caspian.Common.Service;

namespace Caspian.Engine.Service
{
    public class CaspianFontService : BaseService<CaspianFont>
    {
        public CaspianFontService(IServiceProvider provider)
            :base(provider)
        {
            RuleFor(t => t.Name).Required().UniqAsync("فونتی با این عنوان در سیستم ثبت شده است.");
        }
    }
}
