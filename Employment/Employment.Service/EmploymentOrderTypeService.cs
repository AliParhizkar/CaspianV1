using Caspian.Common;
using Employment.Model;
using Caspian.Common.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Employment.Service
{
    public class EmploymentOrderTypeService : SimpleService<EmploymentOrderType>, ISimpleService<EmploymentOrderType>
    {
        public EmploymentOrderTypeService(IServiceScope scope)
            : base(scope)
        {
            RuleFor(t => t.Title).Required().UniqAsync("منطقه ای با این عنوان در این شهر تعریف شده است.");
        }
    }
}
