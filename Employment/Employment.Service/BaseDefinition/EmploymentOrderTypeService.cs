using Caspian.Common;
using Employment.Model;
using Caspian.Common.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Employment.Service
{
    public class EmploymentOrderTypeService : SimpleService<EmploymentOrderType>, ISimpleService<EmploymentOrderType>
    {
        public EmploymentOrderTypeService(IServiceProvider provider)
            : base(provider)
        {
            RuleFor(t => t.Title).Required().UniqAsync("نوع حکمی با این عنوان تعریف شده است");
            RuleFor(t => t.Code).UniqAsync("نوع حکمی با این کد در سیستم تعریف شده است");
            RuleFor(t => t.Description).Required();
        }
    }
}
