using Caspian.Common;
using Caspian.Common.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Caspian.Engine.Service
{
    public class WorkflowFormService : SimpleService<Engine.WorkflowForm>, ISimpleService<Engine.WorkflowForm>
    {
        public WorkflowFormService(IServiceScope scope)
            :base(scope)
        {
            RuleFor(t => t.Title).Required().UniqAsync("فرمی با این عنوان در سیستم ثبت شده است");
            RuleFor(t => t.ColumnCount).CustomValue(t => t < 1, "حداقل یک ستون باید وجود داشته باشد")
                .CustomValue(t => t > 4, "جداکثر چهار ستون می تواند وجود داشته باشد");
        }
    }
}
