using Caspian.UI;
using Caspian.Common;
using System.Reflection;
using Caspian.Common.Service;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace Caspian.Engine.Service
{
    public class DynamicParameterService : SimpleService<DynamicParameter>
    {
        public DynamicParameterService(IServiceScope scope)
            :base(scope)
        {
            RuleFor(t => t.EntityName).Required();
            RuleFor(t => t.FaTitle).Required().UniqAsync("عنوان فارسی باید یکتا باشد.");
            RuleFor(t => t.EnTitle).Required().UniqAsync("عنوان لاتین باید یکتا باشد").CustomValue(t => t.IsValidIdentifire(), "برای تعریف متغیر فقط از حروف لاتین و عدد استفاده نمایید");
            RuleFor(t => t.ControlType).Required(t => t.CalculationType == CalculationType.UserData).Custom(t => t.CalculationType == CalculationType.UserData && t.ControlType != ControlType.CheckBox && t.ControlType != ControlType.Numeric && t.ControlType != ControlType.Integer && t.ControlType != ControlType.DropdownList, "انتخاب این کنترل مجاز نیسیت");
            RuleFor(t => t.ResultType).Required(t => t.CalculationType != CalculationType.UserData);
            RuleFor(t => t.DecimalNumber).Required(t => 
                t.CalculationType == CalculationType.UserData && t.ControlType == ControlType.Numeric || 
                t.CalculationType != CalculationType.UserData && t.ResultType == ResultType.Numeric);
            RuleFor(t => t.RuleId).Required(t => t.CalculationType == CalculationType.Rule);
        }

        public IList<SelectListItem> GetDynamicType(SubSystemKind subSystem)
        {
            var list = new List<SelectListItem>();
            var types = new AssemblyInfo().GetModelTypes(subSystem);
            foreach (var type in types)
            {
                var attr = type.GetCustomAttribute<DynamicTypeAttribute>();
                if (attr != null)
                    list.Add(new SelectListItem(type.Name, attr.Title));
            }
            return list;
        }
    }
}
