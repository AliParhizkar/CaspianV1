using System.Text;
using Caspian.Common;
using System.Reflection;
using Caspian.Common.Service;
using Caspian.Common.Extension;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Engine.Service
{
    public class WorkflowFormService : SimpleService<Engine.WorkflowForm>, ISimpleService<Engine.WorkflowForm>
    {
        public WorkflowFormService(IServiceProvider provider)
            :base(provider)
        {
            RuleFor(t => t.Title).Required().UniqAsync("فرمی با این عنوان در سیستم ثبت شده است");
            RuleFor(t => t.ColumnCount).CustomValue(t => t < 1, "حداقل یک ستون باید وجود داشته باشد")
                .CustomValue(t => t > 4, "جداکثر چهار ستون می تواند وجود داشته باشد");
            RuleFor(t => t.Name).UniqAsync("فرمی با این نام در سیستم ثبت شده است").CustomValue(t => t.IsValidIdentifire(), "برای تعریف کلاس فقط از کاراکترهای لاتین و اعداد استفاده نمایید.");
        }

        string GetDynamicParameterControl(BlazorControl control)
        {
            var param = control.DynamicParameter;
            switch (control.ControlType)
            {
                case ControlType.DropdownList:
                    return "\t\tDropdownList<" + param.EnTitle + "> ddl" + param.EnTitle + ";\n";
                case ControlType.Integer:
                    return "\t\tNumericTextBox<int?> txt" + param.EnTitle + ";\n";
                case ControlType.Numeric:
                    return "\t\tNumericTextBox<decimal?> txt" + param.EnTitle + ";\n";
            }
            throw new NotImplementedException("خطای عدم پیاده سازی");
        }

        string GetCustomFieldControl(BlazorControl control)
        {
            switch (control.ControlType)
            {
                case ControlType.Integer:
                    return "\t\tNumericTextBox<int?> txt" + control.CustomeFieldName + ";\n";
                case ControlType.Numeric:
                    return "\t\tNumericTextBox<decimal?> txt" + control.CustomeFieldName + ";\n";
                case ControlType.String:
                    return "\t\tStringTextBox txt" + control.CustomeFieldName + ";\n";
            }
            throw new NotImplementedException("خطای عدم پیاده سازی");
        }

        public string GetControlType(SubSystemKind subSystem, BlazorControl ctr, bool isServerSide)
        {
            if (ctr.DynamicParameterId.HasValue)
                return GetDynamicParameterControl(ctr);
            else if (ctr.CustomeFieldName.HasValue())
                return GetCustomFieldControl(ctr);
            var entityType = new AssemblyInfo().GetModelType(subSystem, ctr.DataModelField.EntityFullName);
            var info = entityType.GetProperty(ctr.PropertyName);
            switch (ctr.ControlType)
            {
                case ControlType.DropdownList:
                    var enumType = entityType.GetProperty(ctr.PropertyName).PropertyType;
                    var typeName = enumType.GetUnderlyingType().Name;
                    if (enumType.IsNullableType())
                        typeName += "?";
                    return "\t\tDropdownList<" + typeName + "> ddl" + ctr.PropertyName + ";\n";
                case ControlType.ComboBox:
                    var fkInfo = entityType.GetProperties().Single(t => t.GetCustomAttribute<ForeignKeyAttribute>()?.Name == ctr.PropertyName);
                    var str = "\t\tComboBox<" + fkInfo.PropertyType.Name;
                    if (isServerSide)
                    {
                        str += ", " + info.PropertyType.GetUnderlyingType().Name;
                        if (info.PropertyType.IsNullableType())
                            str += "?";
                    }
                    return str + ">cmb" + fkInfo.Name + ";\n";
                case ControlType.String:
                    return "\t\tStringTextBox txt" + ctr.PropertyName + ";\n";
                case ControlType.Date:
                    str = "\t\tDatePicker";
                    if (isServerSide)
                    {
                        str += "<DateTime";
                        if (info.PropertyType.IsNullableType())
                            str += "?";
                        str += ">";
                    }
                    return str + " dpk" + ctr.PropertyName + ";\n";
                case ControlType.Integer:
                case ControlType.Numeric:
                    str = "\t\tNumericTextBox<" + info.PropertyType.GetUnderlyingType();
                    if (info.PropertyType.IsNullableType())
                        str += "?";
                    return str + "> txt" + ctr.PropertyName + ";\n";
                default:
                    throw new NotImplementedException("خطای عدم پیاده سازی");
            }
        }

        public async Task<string> GetCodebehindAsync(int workflowFormId, int dataModelId)
        {
            var str = new StringBuilder();
            var form = await GetAll().Include(t => t.WorkflowGroup).SingleAsync(workflowFormId);
            str.Append("/// This code is generated by caspian generator and can not be changed\n\n");
            str.Append("using System;\n");
            str.Append("using " + form.WorkflowGroup.SubSystemKind.ToString() + ".Model;\n");
            str.Append("using Capian.Dynamicform.Component;\n\n");
            str.Append("namespace Caspian.Dynamic.WorkflowForm\n{\n");
            str.Append("\tpublic partial class " + form.Name + ": BasePage\n\t{\n");
            str.Append("\t\t/// Fields of form\n");
            var fields = await new DataModelFieldService(ServiceProvider).GetAll().Where(t => t.DataModelId == dataModelId).ToListAsync();
            foreach(var field in fields)
            {
                var typeName = field.EntityFullName ?? DataModelFieldService.GetControlTypeName(field.FieldType.Value);
                str.Append("\t\t" + typeName + " " + field.FieldName + ";\n");
            }
            var rows = await new HtmlRowService(ServiceProvider).GetAll().Where(t => t.WorkflowFormId == workflowFormId)
                .Include("Columns").Include("Columns.Component").Include("Columns.Component.DynamicParameter")
                .Include("Columns.Component.DynamicParameter.Options").Include("Columns.Component.DataModelField")
                .Include("Columns.InnerRows").Include("Columns.InnerRows.HtmlColumns")
                .Include("Columns.InnerRows.HtmlColumns.Component")
                .Include("Columns.InnerRows.HtmlColumns.Component.DataModelField")
                .Include("Columns.InnerRows.HtmlColumns.Component.DynamicParameter")
                .Include("Columns.InnerRows.HtmlColumns.Component.DynamicParameter.Options").ToListAsync();
            var controls = new List<BlazorControl>();
            foreach (var row in rows)
                foreach(var col in row.Columns)
                {
                    var ctr = col.Component;
                    if (ctr != null && ctr.DynamicParameter != null)
                        controls.Add(ctr);
                    foreach(var row1 in col.InnerRows)
                    {
                        foreach(var col1 in row1.HtmlColumns)
                        {
                            var ctr1 = col1.Component;
                            if (ctr1 != null && ctr1.DynamicParameter != null)
                                controls.Add(ctr1);
                        }
                    }
                }
            foreach (var control in controls)
            {
                str.Append("\t\t ");
                var param = control!.DynamicParameter;
                var enTitle = param!.EnTitle;
                switch (control.ControlType)
                {
                    case ControlType.Integer:
                        str.Append("int? " + enTitle + ";\n");
                        break;
                    case ControlType.DropdownList:
                        str.Append(enTitle + "? " + enTitle + ";\n");
                        break;
                    default:
                        throw new NotImplementedException("خطای عدم پیاده سازی");
                }
            }
            str.Append("\n\t\t/// Controls of form\n");
            foreach(var row in rows)
                foreach(var col in row.Columns)
                {
                    var ctr = col.Component;
                    if (ctr != null)
                        str.Append(GetControlType(form.WorkflowGroup.SubSystemKind, ctr, false));
                    foreach(var row1 in col.InnerRows)
                    {
                        foreach (var col1 in row1.HtmlColumns)
                        {
                            var ctr1 = col1.Component;
                            if (ctr1 != null)
                                str.Append(GetControlType(form.WorkflowGroup.SubSystemKind, ctr1, false));
                        }
                    }
                }
            str.Append("\t}\n");
            foreach(var control in controls)
            {
                var param = control.DynamicParameter;
                if (param.ControlType == ControlType.DropdownList)
                {
                    str.Append("\n\tinternal enum " + param.EnTitle + "\n\t{\n");
                    var index = 1;
                    foreach(var option in param.Options)
                    {
                        str.Append("\t\t/// <summary>\n");
                        str.Append("\t\t/// " + option.FaTitle + "\n");
                        str.Append("\t\t/// </summary>\n");
                        str.Append("\t\t" + option.EnTitle + " = " + index);
                        if (index < param.Options.Count)
                            str.Append(",\n");
                        str.Append("\n");
                        index++;
                    }
                    str.Append("\t}\n");
                }
            }
            str.Append("}");
            return str.ToString();
        }

        public async Task<string> GetSourceCode(string basePath, int workflowFormId)
        {
            var form = await GetAll().Include(t => t.WorkflowGroup).SingleAsync(workflowFormId);
            if (form.SourceFileName.HasValue())
            {
                var path = basePath + "Data\\Code\\" + form.SourceFileName + ".cs";
                var result = await File.ReadAllTextAsync(path);
                if (result.HasValue())
                    return await File.ReadAllTextAsync(path);
            }
            var str = new StringBuilder();
            str.Append("using " + form.WorkflowGroup.SubSystemKind.ToString() + ".Model;\n");
            str.Append("using Capian.Dynamicform.Component;\n\n");
            str.Append("namespace Caspian.Dynamic.WorkflowForm\n{\n");
            str.Append("\tpublic partial class " + form.Name + "\n\t{\n");
            str.Append("\t\tpublic void Initialize()\n\t\t{\n\n");
            str.Append("\t\t}\n");
            str.Append("\t}\n}");
            return str.ToString();
        }

        public async override Task Remove(int id)
        {
            //await base.Remove(id);
            //var fields = new WfFormEntityFieldService(ServiceScope).GetAll().Where(t => t.WorkflowFormId == id);
            //new WfFormEntityFieldService(ServiceScope).RemoveRange(fields);
            var contextId = Context.ContextId;
            var controls = new BlazorControlService(ServiceProvider).GetAll().Where(t => t.HtmlColumn.Row.WorkflowFormId == id || t.HtmlColumn.InnerRow.HtmlColumn.Row.WorkflowFormId == id);
            new BlazorControlService(ServiceProvider).RemoveRange(controls);
            var columns = new HtmlColumnService(ServiceProvider).GetAll().Where(t => t.Row.WorkflowFormId == id || t.InnerRow.HtmlColumn.Row.WorkflowFormId == id);
            new HtmlColumnService(ServiceProvider).RemoveRange(columns);
            var rows = new HtmlRowService(ServiceProvider).GetAll().Where(t => t.WorkflowFormId == id);
            new HtmlRowService(ServiceProvider).RemoveRange(rows);
            var innerRows = new InnerRowService(ServiceProvider).GetAll().Where(t => t.HtmlColumn.Row.WorkflowFormId == id);
            new InnerRowService(ServiceProvider).RemoveRange(innerRows);
        }
    }
}
