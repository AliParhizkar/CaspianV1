using Caspian.Common;
using Employment.Model;
using Caspian.Common.Service;

namespace Employment.Service
{
    public class EducationDegreeService : BaseService<EducationDegree>, IBaseService<EducationDegree>
    {
        public EducationDegreeService(IServiceProvider provider)
            : base(provider)
        {
            RuleFor(t => t.Title).Required().UniqAsync("مدرکی با این عنوان در سیستم تعریف شده است.");
        }
    }
}
