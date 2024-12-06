using System;
using Demo.Model;
using Caspian.Common;
using FluentValidation;
using Caspian.Common.Service;

namespace Demo.Service
{
    public class EmployeeService: BaseService<Employee>, IBaseService<Employee>
    {
        public EmployeeService(IServiceProvider provider):
            base(provider) 
        {
            RuleFor(t => t.LName).Required();
        }
    }

    public class CourseStudyService : BaseService<CourseStudy>, IBaseService<CourseStudy>
    {
        public CourseStudyService(IServiceProvider provider) :
            base(provider)
        {
            RuleFor(t => t.Title).Required().UniqAsync("پایه تحصیلی با این عنوان در سیستم تعریف شده است");
        }
    }

    public class FamilyService : BaseService<Family>, IBaseService<Family>
    {
        public FamilyService(IServiceProvider provider) :
            base(provider)
        {
            RuleFor(t => t.WifeName).Required();
        }
    }
}
