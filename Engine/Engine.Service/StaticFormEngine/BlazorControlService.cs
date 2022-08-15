﻿using Caspian.Common;
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
            RuleFor(t => t.Caption).Required().UniqAsync("کنترلی با این عنوان در سیستم ثبت شده است");
        }

        string GetId(BlazorControl control)
        {
            switch(control.ControlType)
            {
                case ControlType.Integer:
                case ControlType.Numeric:
                    return "txt" + control.DynamicParameter.EnTitle;
                case ControlType.DropdownList:
                    return "ddl" + control.DynamicParameter.EnTitle;
                default:
                    throw new NotImplementedException("خطای عدم پیاده سازی");
            }
        }

        public async Task<string> GetId(SubSystemKind subSystemKind, BlazorControl ctr)
        {
            if (ctr.DynamicParameterId.HasValue)
                return GetId(ctr);
            switch (ctr.ControlType)
            {
                case ControlType.CheckBox:
                case ControlType.TreeStateCheckBox:
                    return "chb" + ctr.PropertyName;
                case ControlType.ComboBox:
                    if (ctr.DataModelFieldId == 0)
                        ctr.DataModelField = await new DataModelFieldService(ServiceScope).SingleAsync(ctr.DataModelFieldId);
                    var entityType = new AssemblyInfo().GetModelType(subSystemKind, ctr.DataModelField.EntityFullName);
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
