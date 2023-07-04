﻿using Caspian.Common;
using Caspian.Common.Service;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Caspian.Engine.Service
{
    public class BlazorControlService : BaseService<BlazorControl>
    {
        public BlazorControlService(IServiceProvider provider)
            :base(provider)
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
                case ControlType.List:
                    if (ctr.DataModelField == null)
                        ctr.DataModelField = await new DataModelFieldService(ServiceProvider).SingleAsync(ctr.DataModelFieldId);
                    if (ctr.DataModelField.FieldType == DataModelFieldType.Relational)
                    {
                        var name = ctr.DataModelField.FieldName;
                        if (name.EndsWith("Id"))
                            name = name.Substring(0, name.Length - "Id".Length);
                        if (ctr.LookupTypeId.HasValue)
                            return "lkp" + name;
                        return "cmb" + name;
                    }
                    var entityType = new AssemblyInfo().GetModelType(subSystemKind, ctr.DataModelField.EntityFullName);
                    var info = entityType.GetProperties().SingleOrDefault(t => t.GetCustomAttribute<ForeignKeyAttribute>()?.Name == ctr.PropertyName);
                    if (info == null)
                        throw new CaspianException($"خطا: In type {entityType.Name} property with name {ctr.PropertyName} not exist");
                    if (ctr.LookupTypeId.HasValue)
                        return "lkp" + info.Name;
                    return "cmb" + info.Name;
                case ControlType.Date:
                    return "dte" + ctr.PropertyName;
                case ControlType.String:
                case ControlType.Integer:
                case ControlType.Numeric:
                    return "txt" + (ctr.PropertyName ?? ctr.CustomeFieldName);
                case ControlType.DropdownList:
                    return "ddl" + ctr.PropertyName;
                default:
                    throw new NotImplementedException("خطای عدم پیاده سازی");
            }
        }
    }
}
