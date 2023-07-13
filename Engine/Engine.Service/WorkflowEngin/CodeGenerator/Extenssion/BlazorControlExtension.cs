using System.Text;
using Caspian.Common;
using System.Reflection;
using Caspian.Common.Extension;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Engine.Service
{
    public static class BlazorControlExtension
    {
        public static string GetParameterControlCode(this BlazorControl control, bool? isAsync)
        {
            StringBuilder str = new StringBuilder();
            var parameterName = control.DataModelField?.FieldType == DataModelFieldType.MultiOptions ?
                control.DataModelField.FieldName : control.CustomeFieldName ?? control.DataModelField.FieldName;
            switch (control.ControlType)
            {
                case ControlType.Integer:
                case ControlType.Numeric:
                case ControlType.String:
                    var strType = "";
                    if (control.ControlType == ControlType.String)
                    {
                        strType = "string";
                        str.Append("\t\t\tbuilder.OpenComponent<StringTextBox>(2);\n");
                    }
                    else
                    {
                        strType = control.ControlType == ControlType.Integer ? "int?" : "decimal?";
                        str.Append($"\t\t\tbuilder.OpenComponent<NumericTextBox<{strType}>>(2);\n");
                    }
                    ///Value Binding
                    str.Append($"\t\t\tbuilder.AddAttribute(3, \"Value\", {parameterName});\n");
                    str.Append($"\t\t\tbuilder.AddAttribute(3, \"ValueChanged\", EventCallback.Factory.Create<{strType}>(this,");
                    if (isAsync == true)
                        str.Append("async");
                    str.Append($" value => {{\n{parameterName} = value;\n"); ;
                    if (control.OnChange.HasValue())
                    {
                        if (isAsync == true)
                            str.Append($"\t\t\t\tawait {control.OnChange}();\n");
                        else
                            str.Append($"\t\t\t\t{control.OnChange}();\n");
                    }
                    str.Append("}));\n");
                    ///Add Refrence to control
                    str.Append("\t\t\tbuilder.AddComponentReferenceCapture(1, txt =>\n");
                    str.Append("\t\t\t{\n");
                    if (control.ControlType == ControlType.String)
                        str.Append($"\t\t\t\ttxt{parameterName} = txt as StringTextBox;\n");
                    else
                        str.Append($"\t\t\t\ttxt{parameterName} = txt as NumericTextBox<{strType}>;\n");
                    str.Append("\t\t\t});\n");
                    //-----------------------------------------
                    str.Append("\t\t\tbuilder.CloseComponent();\n");
                    break;
                case ControlType.List:
                    var typeName = control.DataModelField.EntityType.Name;
                    str.Append($"\t\t\tbuilder.OpenComponent<ComboBox<{typeName}, int?>>(2);\n");
                    str.Append($"\t\t\tbuilder.AddAttribute(3, \"Value\", {control.DataModelField.FieldName});\n");
                    str.Append($"\t\t\tbuilder.AddAttribute(3, \"ValueChanged\", EventCallback.Factory.Create<int?>(this, value => {{{control.DataModelField.FieldName} = value; }}));\n");
                    if (control.OnChange.HasValue())
                    {
                        if (isAsync == true)
                            str.Append($"\t\t\tbuilder.AddAttribute(3, \"OnChanged\", EventCallback.Factory.Create(this, async () => await {control.OnChange}()));\n");
                        else
                            str.Append($"\t\t\tbuilder.AddAttribute(3, \"OnChanged\", EventCallback.Factory.Create(this, () => {control.OnChange}()));\n");
                    }
                    str.Append("\t\t\tbuilder.AddComponentReferenceCapture(1, cmb =>\n");
                    str.Append("\t\t\t{\n");
                    var strName = parameterName;
                    if (parameterName.EndsWith("Id"))
                        strName = parameterName.Substring(0, strName.Length - "Id".Length);
                    strName = $"cmb{strName}";
                    str.Append($"\t\t\t\t{strName} = cmb as ComboBox<{typeName}, int?>;\n");
                    if (control.TextExpression.HasValue())
                        str.Append($"\t\t\t\t{strName}.TextExpression = {control.TextExpression};\n");
                    if (control.ConditionExpression.HasValue())
                        str.Append($"\t\t\t\t{strName}.ConditionExpression = {control.ConditionExpression};\n");
                    str.Append("\t\t\t});\n");
                    str.Append("\t\t\tbuilder.CloseComponent();\n");
                    break;
                case ControlType.DropdownList:
                    strType = parameterName;
                    str.Append($"\t\t\tbuilder.OpenComponent<DropdownList<{strType}>>(2);\n");
                    ///Value Binding
                    str.Append($"\t\t\tbuilder.AddAttribute(3, \"Value\", {parameterName});\n");
                    str.Append($"\t\t\tbuilder.AddAttribute(3, \"ValueChanged\", EventCallback.Factory.Create<{strType}>(this,");
                    if (isAsync == true)
                        str.Append("async");

                    str.Append($" value => {{\n{parameterName} = value;\n");
                    if (control.OnChange.HasValue())
                    {
                        if (isAsync == true)
                            str.Append($"\t\t\t\tawait {control.OnChange}();\n");
                        else
                            str.Append($"\t\t\t\t{control.OnChange}();\n");
                    }
                    str.Append("}));\n");
                    ///Add Refrence to control
                    str.Append("\t\t\tbuilder.AddComponentReferenceCapture(1, ddl =>\n");
                    str.Append("\t\t\t{\n");
                    str.Append($"\t\t\t\tddl{parameterName} = ddl as DropdownList<{strType}>;\n");
                    str.Append("\t\t\t});\n");
                    str.Append("\t\t\tbuilder.CloseComponent();\n");
                    break;
                default:
                    throw new NotImplementedException("خطای عدم پیاده سازی");
            }
            return str.ToString();
        }

        public static string GetCode(this BlazorControl component, string formName, SubSystemKind kind, string userCode)
        {
            var str = new StringBuilder();
            bool? isAsync = null;
            if (component.OnChange.HasValue())
                isAsync = new CodeManager().MethodIsAsync(formName, component!.OnChange, userCode);
            str.Append("\t\t\tbuilder.OpenElement(1, \"div\");\n");
            str.Append("\t\t\tbuilder.AddAttribute(3, \"class\", \"c-dynamic-form-controls\");\n");
            if (component.MultiLine && component.Height > 1)
            {
                var style = $"height:{(component.Height - 1) * 80 + 61}px";
                str.Append($"\t\t\tbuilder.AddAttribute(3, \"style\", \"{style}\");");
            }
            str.Append("\t\t\tbuilder.OpenElement(1, \"label\");\n");
            str.Append("\t\t\tbuilder.AddAttribute(3, \"class\", \"pe-3\");\n");
            str.Append($"\t\t\tbuilder.AddContent(1, \"{component.Caption}\");\n");
            str.Append("\t\t\tbuilder.CloseElement();\n");
            if (component.CustomeFieldName.HasValue() || component.DataModelField?.FieldType == DataModelFieldType.MultiOptions)
                str.Append(component.GetParameterControlCode(isAsync));
            else
            {
                var type = new AssemblyInfo().GetModelType(kind, component.DataModelField.EntityFullName);
                var info = type.GetProperty(component.PropertyName);
                type = info!.PropertyType;
                var strType = type.GetUnderlyingType().Name;
                if (type.IsNullableType())
                    strType += "?";
                var id = component.GetId(kind);
                switch (component.ControlType)
                {
                    case ControlType.String:
                        str.Append("\t\t\tbuilder.OpenComponent<StringTextBox>(2);\n");
                        if (component.MultiLine && component.Height > 1)
                        {
                            var style = $"height:{(component.Height - 1) * 62 + 29}px";
                            str.Append($"\t\t\tbuilder.AddAttribute(3, \"style\", \"{style}\");");
                            str.Append("\t\t\tbuilder.AddAttribute(3, \"MultiLine\", true);");
                        }
                        str.Append($"\t\t\tbuilder.AddAttribute(3, \"Value\", {component.DataModelField.FieldName}.{component.PropertyName});\n");
                        str.Append($"\t\t\tbuilder.AddAttribute(3, \"ValueChanged\", EventCallback.Factory.Create<string>(this, value => {{{component.DataModelField.FieldName}.{component.PropertyName} = value; }}));\n");
                        str.Append("\t\t\tbuilder.CloseComponent();\n");
                        break;
                    case ControlType.DropdownList:
                        str.Append($"\t\t\tbuilder.OpenComponent<DropdownList<{strType}>>(2);\n");
                        str.Append($"\t\t\tbuilder.AddAttribute(3, \"Value\", {component.DataModelField.FieldName}.{component.PropertyName});\n");
                        str.Append($"\t\t\tbuilder.AddAttribute(3, \"ValueChanged\", EventCallback.Factory.Create<{strType}>(this,");
                        if (isAsync == true)
                            str.Append("async");
                        str.Append($" value => \n\t\t\t{{\n\t\t\t\t{component.DataModelField.FieldName}.{component.PropertyName} = value;\n");
                        if (component.OnChange.HasValue())
                        {
                            if (isAsync == true)
                                str.Append($"\t\t\t\tawait {component.OnChange}();\n");
                            else
                                str.Append($"\t\t\t\t{component.OnChange}();\n");
                        }

                        str.Append("\t\t\t}));\n");
                        str.Append("\t\t\tbuilder.CloseComponent();\n");
                        break;
                    case ControlType.Date:
                        str.Append($"\t\t\tbuilder.OpenComponent<DatePicker<{strType}>>(2);\n");
                        str.Append($"\t\t\tbuilder.AddAttribute(3, \"Value\",{component.DataModelField.FieldName}.{component.PropertyName});\n");
                        str.Append($"\t\t\tbuilder.AddAttribute(3, \"ValueChanged\", EventCallback.Factory.Create<{strType}>(this, value => {{{component.DataModelField.FieldName}.{component.PropertyName} = value; }}));\n");
                        str.Append("\t\t\tbuilder.CloseComponent();\n");
                        break;
                    case ControlType.List:
                        var typeName = info.GetForeignKey().PropertyType.Name;
                        if (component.LookupTypeId.HasValue)
                        {
                            str.Append($"\t\t\tbuilder.OpenComponent<AutoComplete<{typeName}, {strType}>>(2);\n");
                            str.Append($"\t\t\tRenderFragment render{component.LookupTypeId} = t =>\n");
                            str.Append("\t\t\t{\n");
                            str.Append($"\t\t\t\tt.OpenComponent(1, typeof({component.LookupType.LookupTypeName}<{strType}>));\n");
                            str.Append("\t\t\t\tt.CloseComponent();");
                            str.Append("\t\t\t};\n");
                            str.Append($"\t\t\tbuilder.AddAttribute(3, \"ChildContent\", render{component.LookupTypeId});\n");
                        }
                        else
                            str.Append($"\t\t\tbuilder.OpenComponent<ComboBox<{typeName}, {strType}>>(2);\n");
                        str.Append($"\t\t\tbuilder.AddAttribute(3, \"Value\", {component.DataModelField.FieldName}.{component.PropertyName});\n");
                        str.Append($"\t\t\tbuilder.AddAttribute(3, \"ValueChanged\", EventCallback.Factory.Create<{strType}>(this, value => {{{component.DataModelField.FieldName}.{component.PropertyName} = value; }}));\n");
                        if (component.OnChange.HasValue())
                        {
                            if (isAsync == true)
                                str.Append($"\t\t\tbuilder.AddAttribute(3, \"OnChange\", EventCallback.Factory.Create(this, async () => await {component.OnChange}()));\n");
                            else
                                str.Append($"\t\t\tbuilder.AddAttribute(3, \"OnChange\", EventCallback.Factory.Create(this, () => {component.OnChange}()));\n");
                        }
                        if (component.LookupTypeId.HasValue)
                            str.Append("\t\t\tbuilder.AddComponentReferenceCapture(1, lkp =>\n");
                        else
                            str.Append("\t\t\tbuilder.AddComponentReferenceCapture(1, cmb =>\n");
                        str.Append("\t\t\t{\n");
                        if (component.LookupTypeId.HasValue)
                            str.Append($"\t\t\t\t{id} = lkp as AutoComplete<{typeName}, {strType}>;\n");
                        else
                            str.Append($"\t\t\t\t{id} = cmb as ComboBox<{typeName}, {strType}>;\n");
                        if (component.TextExpression.HasValue())
                            str.Append($"\t\t\t\t{id}.TextExpression = {component.TextExpression};\n");
                        if (component.ConditionExpression.HasValue() && component.LookupTypeId == null)
                            str.Append($"\t\t\t\t{id}.ConditionExpression = {component.ConditionExpression};\n");
                        str.Append("\t\t\t});\n");
                        str.Append("\t\t\tbuilder.CloseComponent();\n");
                        break;
                    case ControlType.Integer:
                    case ControlType.Numeric:
                        strType = info.PropertyType.GetUnderlyingType().Name;
                        if (info.PropertyType.IsNullableType())
                            strType += "?";
                        str.Append($"\t\t\tbuilder.OpenComponent<NumericTextBox<{strType}>>(2);\n");
                        str.Append($"\t\t\tbuilder.AddAttribute(3, \"Value\", {component.DataModelField.FieldName}.{component.PropertyName});\n");
                        str.Append($"\t\t\tbuilder.AddAttribute(3, \"ValueChanged\", EventCallback.Factory.Create<{strType}>(this, value => {{ {component.DataModelField.FieldName}.{component.PropertyName} = value; }}));\n");

                        str.Append("\t\t\tbuilder.AddComponentReferenceCapture(1, txt =>\n");
                        str.Append("\t\t\t{\n");
                        str.Append($"\t\t\t\t{id} = txt as NumericTextBox<{strType}>;\n");
                        str.Append("\t\t\t});\n");

                        str.Append("\t\t\tbuilder.CloseComponent();\n");
                        break;
                    default:
                        throw new NotImplementedException("خطای عدم پیاده سازی");
                }
            }
            str.Append("\t\t\tbuilder.CloseElement();\n");
            return str.ToString();
        }

        public static string GetCustomFieldControl(this BlazorControl control, bool isServerSide)
        {
            switch (control.ControlType)
            {
                case ControlType.Integer:
                    return $"\t\tNumericTextBox<int?> txt{control.CustomeFieldName};\n";
                case ControlType.Numeric:
                    return $"\t\tNumericTextBox<decimal?> txt{control.CustomeFieldName};\n";
                case ControlType.String:
                    return $"\t\tStringTextBox txt{control.CustomeFieldName};\n";
                case ControlType.List:
                    var str = $"\t\tComboBox<{control.DataModelField.EntityType.Name}";
                    var id = control.CustomeFieldName;
                    if (id.EndsWith("Id"))
                        id = id.Substring(0, id.Length - 2);
                    if (isServerSide)
                        str += ", int?";
                    return str + $"> cmb{id};\n";
                case ControlType.DropdownList:
                    return $"\t\tDropdownList<{control.DataModelField.FieldName}> ddl{control.DataModelField.FieldName};\n";
            }
            throw new NotImplementedException("خطای عدم پیاده سازی");
        }

        public static string GetControlType(this BlazorControl ctr, SubSystemKind subSystem, bool isServerSide)
        {
            if (ctr.CustomeFieldName.HasValue())
                return ctr.GetCustomFieldControl(isServerSide);
            var entityType = new AssemblyInfo().GetModelType(subSystem, ctr.DataModelField.EntityFullName);
            var info = entityType.GetProperty(ctr.PropertyName);
            switch (ctr.ControlType)
            {
                case ControlType.DropdownList:
                    var enumType = entityType.GetProperty(ctr.PropertyName).PropertyType;
                    var typeName = enumType.GetUnderlyingType().Name;
                    if (enumType.IsNullableType())
                        typeName += "?";
                    return $"\t\tDropdownList<{typeName}> ddl{ctr.PropertyName};\n";
                case ControlType.List:
                    var fkInfo = entityType.GetProperties().Single(t => t.GetCustomAttribute<ForeignKeyAttribute>()?.Name == ctr.PropertyName);
                    var str = "";
                    if (ctr.LookupTypeId.HasValue)
                        str = $"\t\tAutoComplete<{fkInfo.PropertyType.Name}";
                    else
                        str = $"\t\tComboBox<{fkInfo.PropertyType.Name}";
                    if (isServerSide)
                    {
                        str += $", {info.PropertyType.GetUnderlyingType().Name}";
                        if (info.PropertyType.IsNullableType())
                            str += "?";
                    }
                    if (ctr.LookupTypeId.HasValue)
                        return $"{str}> lkp{fkInfo.Name};\n";
                    return $"{str}> cmb{fkInfo.Name};\n";
                case ControlType.String:
                    return $"\t\tStringTextBox txt{ctr.PropertyName};\n";
                case ControlType.Date:
                    str = "\t\tDatePicker";
                    if (isServerSide)
                    {
                        str += "<DateTime";
                        if (info.PropertyType.IsNullableType())
                            str += "?";
                        str += ">";
                    }
                    return $"{str} dpk{ctr.PropertyName};\n";
                case ControlType.Integer:
                case ControlType.Numeric:
                    str = $"\t\tNumericTextBox<{info.PropertyType.GetUnderlyingType()}";
                    if (info.PropertyType.IsNullableType())
                        str += "?";
                    return $"{str}> txt{ctr.PropertyName};\n";
                default:
                    throw new NotImplementedException("خطای عدم پیاده سازی");
            }
        }

        public static string GetId(this BlazorControl ctr, SubSystemKind subSystemKind)
        {
            var str = new StringBuilder();
            switch (ctr.ControlType)
            {
                case ControlType.CheckBox:
                case ControlType.TreeStateCheckBox:
                    return $"chb{ctr.PropertyName}";
                case ControlType.List:
                    if (ctr.DataModelField.FieldType == DataModelFieldType.Relational)
                    {
                        var name = ctr.DataModelField.FieldName;
                        if (name.EndsWith("Id"))
                            name = name.Substring(0, name.Length - "Id".Length);
                        if (ctr.LookupTypeId.HasValue)
                            return $"lkp{name}";
                        return $"cmb{name}";
                    }
                    var entityType = new AssemblyInfo().GetModelType(subSystemKind, ctr.DataModelField.EntityFullName);
                    var info = entityType.GetProperties().SingleOrDefault(t => t.GetCustomAttribute<ForeignKeyAttribute>()?.Name == ctr.PropertyName);
                    if (info == null)
                        throw new CaspianException($"خطا: In type {entityType.Name} property with name {ctr.PropertyName} not exist");
                    if (ctr.LookupTypeId.HasValue)
                        return $"lkp{info.Name}";
                    return $"cmb{info.Name}";
                case ControlType.Date:
                    return $"dte{ctr.PropertyName}";
                case ControlType.String:
                case ControlType.Integer:
                case ControlType.Numeric:
                    return $"txt{ctr.PropertyName ?? ctr.CustomeFieldName}";
                case ControlType.DropdownList:
                    if (ctr.DataModelField?.FieldType == DataModelFieldType.MultiOptions)
                        return $"ddl{ctr.DataModelField.FieldName}";
                    return $"ddl{ctr.PropertyName}";
                default:
                    throw new NotImplementedException("خطای عدم پیاده سازی");
            }
        }
    }
}
