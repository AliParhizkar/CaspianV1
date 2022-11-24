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
            RuleFor(t => t.Title).Required().UniqAsync("عنوان فارسی باید یکتا باشد.");
            RuleFor(t => t.EnTitle).Required().UniqAsync("عنوان لاتین باید یکتا باشد").CustomValue(t => t.IsValidIdentifire(), "برای تعریف متغیر فقط از حروف لاتین و عدد استفاده نمایید");
            RuleFor(t => t.ControlType).Required(t => t.CalculationType == CalculationType.UserData).Custom(t => t.CalculationType == CalculationType.UserData && t.ControlType != ControlType.CheckBox && t.ControlType != ControlType.Numeric && t.ControlType != ControlType.Integer && t.ControlType != ControlType.DropdownList, "انتخاب این کنترل مجاز نیسیت");
            RuleFor(t => t.ResultType).Required(t => t.CalculationType != CalculationType.UserData);
            RuleFor(t => t.DecimalNumber).Required(t => 
                t.CalculationType == CalculationType.UserData && t.ControlType == ControlType.Numeric || 
                t.CalculationType != CalculationType.UserData && t.ResultType == ResultType.Numeric);
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

        public int? CalculatePriority(DynamicParameter parameter)
        {
            if (parameter.CalculationType == CalculationType.UserData)
                return 1;
            int? maxPriprity = null;
            foreach(var data in parameter.ResultParameters)
            {
                if (data.DynamicParameter != null)
                {
                    if (data.DynamicParameter.CalculationType != CalculationType.UserData)
                        return null;
                }
                else if (data.Rule != null)
                {
                    if (data.Rule.Priority == null)
                        return null;
                    if (data.Rule.Priority.Value > maxPriprity.GetValueOrDefault())
                        maxPriprity = data.Rule.Priority.Value;
                }
                else if (!data.PropertyName.HasValue())

                    throw new NotImplementedException("خطای عدم پیاده سازی");
            }
            if (maxPriprity == null)
                return 1;
            return maxPriprity + 1;
        }

        public void SetUserParametersDefault(IList<DynamicParameter> parameters, IDictionary<int, object> values)
        {
            foreach (var parameter in parameters)
            {
                if (parameter.CalculationType == CalculationType.UserData)
                {
                    if (parameter.ControlType == ControlType.DropdownList)
                        values.Add(parameter.Id, 1);
                    if (parameter.ControlType == ControlType.CheckBox)
                        values.Add(parameter.Id, false);
                }
            }
        }
    }
}
