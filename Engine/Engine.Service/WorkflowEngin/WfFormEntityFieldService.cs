using Caspian.Common;
using Caspian.Common.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Caspian.Engine.Service
{
    public class WfFormEntityFieldService : SimpleService<WfFormEntityField>
    {
        public WfFormEntityFieldService(IServiceScope scope)
            :base(scope)
        {
            RuleFor(t => t.Title).Required().UniqAsync(t => t.WorkflowFormId, "آبجکتی با این عنوان در فرم ثبت شده است");
            RuleFor(t => t.FieldName).Required().UniqAsync(t => t.WorkflowFormId, "آبجکتی با این عنوان در فرم ثبت شده است")
                .CustomValue(t => t.IsValidIdentifire(), "برای تعریف متغیر فقط از کاراکترهای لاتین و یا اعداد استفاده کنید.");
            RuleFor(t => t.EntityFullName).Required().UniqAsync(t => t.WorkflowFormId, "هر موجودیت تنها یک بار می تواند در فرم  استفاده شود");
        }
    }

    internal static class Stringextenssion
    {
        public static bool IsValidIdentifire(this string str )
        {
            if (!str.HasValue())
                return false;
            var upper = str.ToUpper();
            if (upper[0] < 'A' || upper[0] > 'Z')
                return true;
            var isInvalid = false;
            foreach(var chr in upper)
            {
                if(chr < '0' || chr > '9' && chr < 'A' || chr > 'Z')
                    isInvalid = true;
            }
            return isInvalid;
        }
    }
}
