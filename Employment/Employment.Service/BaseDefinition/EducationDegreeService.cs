using Caspian.Common;
using Employment.Model;
using Caspian.Common.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Employment.Service
{
    public class EducationDegreeService : SimpleService<EducationDegree>, ISimpleService<EducationDegree>
    {
        public EducationDegreeService(ServiceProvider provider)
            : base(provider)
        {
            RuleFor(t => t.Title).Required().UniqAsync("مدرکی با این عنوان در سیستم تعریف شده است.");
        }
    }
}
