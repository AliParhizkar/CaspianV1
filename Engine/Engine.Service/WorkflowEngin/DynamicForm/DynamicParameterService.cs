using Caspian.UI;
using Caspian.Common;
using System.Reflection;
using Caspian.Common.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Caspian.Engine.Service
{
    public class DynamicParameterService : SimpleService<DynamicParameter>
    {
        public DynamicParameterService(IServiceScope scope)
            :base(scope)
        {
            RuleFor(t => t.ControlType).CustomValue(t => t != ControlType.Numeric && t != ControlType.Integer && t != ControlType.DropdownList, "انتخاب این کنترل مجاز نیسیت");
            RuleFor(t => t.EntityName).Required();
            RuleFor(t => t.FaTitle).Required().UniqAsync("عنوان فارسی باید یکتا باشد.");
            RuleFor(t => t.EnTitle).Required().UniqAsync("عنوان لاتین باید یکتا باشد").CustomValue(t => t.IsValidIdentifire(), "برای تعریف متغیر فقط از حروف لاتین و عدد استفاده نمایید");
            RuleFor(t => t.DecimalNumber).Required(t => t.ControlType == ControlType.Numeric);
        }

        public List<SelectListItem> GetDynamicType(SubSystemKind subSystem)
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
