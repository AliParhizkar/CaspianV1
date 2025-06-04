using System;
using Demo.Model;
using Caspian.Common;
using Caspian.Common.Service;

namespace Demo.Service
{
    public class CourseStudyService : BaseService<CourseStudy>
    {
        public CourseStudyService(IServiceProvider provider)
            : base(provider)
        {
            RuleFor(t => t.Title).Required().UniqueAsync(t => t.EmployeeId, "دوره ای با این عنوان برای کامند ثبت شده است");
        }
    }
}
