using Caspian.Common;
using Caspian.Common.Service;

namespace Procurement.Service
{
    public class ServiceService : BaseService<Model.Service>
    {
        public ServiceService(IServiceProvider provider)
            :base(provider)
        {
            RuleFor(t => t.Code).Required().UniqueAsync("خدمتی با این کد در سیستم ثبت شده است");
            RuleFor(t => t.Name).Required().UniqueAsync("خدمتی با این نام در سیستم ثبت شده است");
        }
    }
}
