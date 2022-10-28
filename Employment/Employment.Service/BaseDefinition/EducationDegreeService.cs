using Caspian.Common;
using Employment.Model;
using Caspian.Common.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Employment.Service
{
    public class EducationDegreeService : SimpleService<EducationDegree>, ISimpleService<EducationDegree>
    {
        public EducationDegreeService(IServiceScope scope)
            : base(scope)
        {
            RuleFor(t => t.Title).Required().UniqAsync("مدرکی با این عنوان در سیستم تعریف شده است.");
        }
    }
}
