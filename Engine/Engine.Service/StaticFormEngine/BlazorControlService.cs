using Caspian.Common;
using Caspian.Common.Service;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Caspian.Engine.Service
{
    public class BlazorControlService : SimpleService<BlazorControl>
    {
        public BlazorControlService(IServiceScope scope)
            :base(scope)
        {

        }

        public async Task<string> GetId(SubSystemKind subSystemKind, BlazorControl ctr)
        {
            switch (ctr.ControlType)
            {
                case ControlType.CheckBox:
                case ControlType.TreeStateCheckBox:
                    return "chb" + ctr.PropertyName;
                case ControlType.ComboBox:
                    if (ctr.WfFormEntityField == null)
                        ctr.WfFormEntityField = await new WfFormEntityFieldService(ServiceScope).SingleAsync(ctr.WfFormEntityFieldId!.Value);
                    var entityType = new AssemblyInfo().GetModelType(subSystemKind, ctr.WfFormEntityField.EntityFullName);
                    var info = entityType.GetProperties().Single(t => t.GetCustomAttribute<ForeignKeyAttribute>()?.Name == ctr.PropertyName);
                    return "cmb" + info.Name;
                case ControlType.Date:
                    return "dte" + ctr.PropertyName;
                case ControlType.String:
                case ControlType.Integer:
                case ControlType.Numeric:
                    return "txt" + ctr.PropertyName;
                case ControlType.DropdownList:
                    return "ddl" + ctr.PropertyName;
                default:
                    throw new NotImplementedException("خطای عدم پیاده سازی");
            }
        }
    }
}
