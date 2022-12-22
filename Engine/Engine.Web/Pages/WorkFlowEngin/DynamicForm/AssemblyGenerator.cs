using Caspian.UI;
using System.Text;
using Caspian.Common;
using FluentValidation;
using System.Reflection;
using System.Runtime.Loader;
using Caspian.Engine.Service;
using Microsoft.CodeAnalysis;
using Caspian.Common.Extension;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.EntityFrameworkCore;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.AspNetCore.Components;

namespace Caspian.Engine.WorkflowEngine
{
    public partial class AssemblyGenerator: BasePage 
    {
        WorkflowForm form;
        Type? DynamicType;
        bool isloading = false;
        string errorMessage;

        protected override async Task OnInitializedAsync()
        {
            if (DynamicType == null)
            {
                if (!isloading)
                {
                    using var scope = CreateScope();
                    var service = new WorkflowFormService(scope.ServiceProvider);
                    form = await service.GetAll().Include("Rows").Include("WorkflowGroup").Include("Rows.Columns")
                        .Include("Rows.Columns.Component").Include("Rows.Columns.Component.DataModelField")
                        .Include("Rows.Columns.Component.DynamicParameter")
                        .Include("Rows.Columns.Component.DynamicParameter.Options")
                        .Include("Rows.Columns.InnerRows").Include("Rows.Columns.InnerRows.HtmlColumns")
                        .Include("Rows.Columns.InnerRows.HtmlColumns.Component")
                        .Include("Rows.Columns.InnerRows.HtmlColumns.Component.DataModelField")
                        .Include("Rows.Columns.InnerRows.HtmlColumns.Component.DynamicParameter")
                        .Include("Rows.Columns.InnerRows.HtmlColumns.Component.DynamicParameter.Options")
                        .SingleAsync(t => t.Id == WorkflowFormId);
                    isloading = true;
                    try
                    {
                        var userSource = GetSourceFile();
                        userSource += CreateDynamicOptionCode();
                        
                        var strSource = await CreateCodebehindFormFile(userSource);
                        CreateAssembly(strSource);
                        errorMessage = null;
                    }
                    catch(CaspianException ex)
                    {
                        errorMessage = ex.Message;
                    }
                }
            }
            await base.OnInitializedAsync();
        }

        string? GetSourceFile()
        {
            if (form.SourceFileName.HasValue())
            {
                var path = Environment.ContentRootPath + "Data\\Code\\" + form.SourceFileName + ".cs";
                var content =  File.ReadAllText(path);
                if (!content.HasValue())
                    return null;
                return new CodeManager().FindSourceCode(content);
            }
            return null;
        }

        string CreateDynamicOptionCode()
        {
            StringBuilder str = new StringBuilder();
            var list = new List<DynamicParameter>();
            foreach (var row in form.Rows)
                foreach (var col in row.Columns)
                {
                    var param = col.Component?.DynamicParameter;
                    if (param != null && param.ControlType == ControlType.DropdownList)
                        list.Add(param);
                    foreach(var row1 in col.InnerRows)
                        foreach(var col1 in row1.HtmlColumns)
                        {
                            var param1 = col1.Component?.DynamicParameter;
                            if (param1 != null && param1.ControlType == ControlType.DropdownList)
                                list.Add(param1);
                        }
                }
            foreach(var param in list)
            {
                str.Append("\tpublic enum " + param.EnTitle + "\n\t{\n");
                var index = 1;
                foreach (var option in param.Options)
                {
                    str.Append("\t\t[EnumField(\"" + option.FaTitle + "\")]\n");
                    str.Append("\t\t" + option.EnTitle + " = " + index);
                    if (index != param.Options.Count)
                        str.Append(",\n");
                    str.Append("\n");
                    index++;
                }
                str.Append("\t}");
            }
            return str.ToString();
        }

        async Task<string> CreateCodebehindFormFile(string userCode)
        {
            using var scope = CreateScope();
            var service = new WorkflowFormService(scope.ServiceProvider);
            var fields = new DataModelFieldService(scope.ServiceProvider).GetAll().Where(t => t.DataModelId == form.DataModelId).ToList();
            var str = new StringBuilder();
            str.Append("using Caspian.UI;\n");
            str.Append("using System;\n");
            str.Append("using Caspian.Common.Attributes;\n");
            str.Append("using " + form.WorkflowGroup.SubSystemKind.ToString() + ".Model;\n");
            str.Append("using " + form.WorkflowGroup.SubSystemKind.ToString() + ".Service;\n");
            str.Append("using Microsoft.AspNetCore.Components;\n");
            str.Append("using Caspian.Common;\n");
            str.Append("using System.Threading.Tasks;\n");
            str.Append("using Microsoft.AspNetCore.Components.Rendering;\n\n");
            str.Append("namespace Caspian.Engine.CodeGenerator\n{\n\t");
            str.Append("public partial class " + form.Name + ": BasePage\n");
            str.Append("\t{\n");
            foreach (var field in fields)
            {
                var typeName = "";
                if (field.EntityFullName.HasValue())
                    typeName = field.EntityFullName;
                else
                    typeName = DataModelFieldService.GetControlTypeName(field.FieldType.Value);
                str.Append("\t\t" + typeName + ' ' + field.FieldName + ";\n");
            }
            str.Append("\n\t\t//Dynamic parameters\n");
            var list = new List<DynamicParameter>();
            foreach (var row in form.Rows)
                foreach (var col in row.Columns)
                {
                    var param = col.Component?.DynamicParameter;
                    if (param != null)
                        list.Add(param);
                    foreach(var row1 in col.InnerRows)
                        foreach(var col1 in row1.HtmlColumns)
                        {
                            param = col1.Component?.DynamicParameter;
                            if (param != null)
                                list.Add(param);
                        }
                }
            foreach(var param in list)
            {
                str.Append("\t\t");
                switch (param.ControlType)
                {
                    case ControlType.Integer:
                        str.Append("int? ");
                        break;
                    case ControlType.Numeric:
                        str.Append("decimal? ");
                        break;
                    case ControlType.DropdownList:
                        str.Append(param.EnTitle + "? ");
                        break;
                }
                str.Append(param.EnTitle + ";\n");
            }

            foreach (var row in form.Rows)
                foreach(var col in row.Columns)
                {
                    var ctr = col.Component;
                    if (ctr != null)
                        str.Append(service.GetControlType(form.WorkflowGroup.SubSystemKind, ctr, true));
                    foreach (var row1 in col.InnerRows)
                        foreach(var col1 in row1.HtmlColumns)
                        {
                            ctr = col1.Component;
                            if (ctr != null)
                                str.Append(service.GetControlType(form.WorkflowGroup.SubSystemKind, ctr, true));
                        }
                }
            str.Append("\n");
            str.Append("\t\tprotected override void OnInitialized()\n\t\t{\n");
            foreach(var field in fields)
            {
                if (field.EntityFullName.HasValue())
                    str.Append("\t\t\t" + field.FieldName + " = new " + field.EntityFullName + "();\n");
            }
            str.Append("\t\t\tbase.OnInitialized();\n");
            str.Append("\t\t}\n\n");
            str.Append("\t\tprotected override void BuildRenderTree(RenderTreeBuilder builder)\n\t\t{\n");
            //Create MessageBox Component
            str.Append("\t\t\tbuilder.OpenComponent<MessageBox>(2);\n");
            str.Append("\t\t\tbuilder.AddComponentReferenceCapture(1, msg => { MessageBox = msg as MessageBox; });\n");
            str.Append("\t\t\tbuilder.CloseComponent();");
            foreach (var row in form.Rows)
            {
                str.Append("\t\t\tbuilder.OpenElement(1, \"div\");\n");
                str.Append("\t\t\tbuilder.AddAttribute(3, \"class\", \"row\");\n");
                foreach (var col in row.Columns)
                {
                    str.Append("\t\t\tbuilder.OpenElement(1, \"div\");\n");
                    str.Append("\t\t\tbuilder.AddAttribute(3, \"class\", \"col-md-" + col.Span + "\");\n");
                    if (col.Component != null)
                        await CreateControl(col.Component, str, userCode);
                    else if (col.InnerRows != null)
                    {
                        foreach(var row1 in col.InnerRows)
                        {
                            str.Append("\t\t\tbuilder.OpenElement(1, \"div\");\n");
                            str.Append("\t\t\tbuilder.AddAttribute(3, \"class\", \"row\");\n");
                            foreach(var col1 in row1.HtmlColumns)
                            {
                                str.Append("\t\t\tbuilder.OpenElement(1, \"div\");\n");
                                str.Append("\t\t\tbuilder.AddAttribute(3, \"class\", \"col-md-" + col1.Span + "\");\n");
                                if (col1.Component != null)
                                    await CreateControl(col1.Component, str, userCode);
                                str.Append("\t\t\tbuilder.CloseElement();\n");
                            }
                            str.Append("\t\t\tbuilder.CloseElement();\n");
                        }
                    }
                    str.Append("\t\t\tbuilder.CloseElement();\n");
                }
                str.Append("\t\t\tbuilder.CloseElement();\n");
            }
            str.Append("\t\t}\n\t}");
            if (userCode.HasValue())
                str.Append("\n\n" + userCode);
            str.Append("\n}");
            return str.ToString();
        }

        void CreateParameterControl(BlazorControl control, StringBuilder str, string userCode)
        {
            str.Append("\t\t\tbuilder.OpenElement(1, \"fieldset\");\n");
            str.Append("\t\t\tbuilder.AddAttribute(3, \"class\", \"c-dynamic-form-controls\");\n");
            str.Append("\t\t\tbuilder.OpenElement(1, \"legend\");\n");
            str.Append("\t\t\tbuilder.AddContent(1, \"" + control.Caption + "\");\n");
            str.Append("\t\t\tbuilder.CloseElement();\n");
            bool? isAsync = null;
            if (control.OnChange.HasValue())
                isAsync = new CodeManager().MethodIsAsync(form.Name, control!.OnChange, userCode);
            var parameterName = control.CustomeFieldName ?? control?.DynamicParameter?.EnTitle ?? control.DataModelField.FieldName;
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
                        str.Append("\t\t\tbuilder.OpenComponent<NumericTextBox<" + strType + ">>(2);\n");
                    }
                    ///Value Binding
                    str.Append("\t\t\tbuilder.AddAttribute(3, \"Value\", " + parameterName + ");\n");
                    str.Append("\t\t\tbuilder.AddAttribute(3, \"ValueChanged\", EventCallback.Factory.Create<" + strType + ">(this," + (isAsync == true ? "async" : "") + " value => {\n" + parameterName + " = value;\n");
                    if (control.OnChange.HasValue())
                    {
                        if (isAsync == true)
                            str.Append("\t\t\t\tawait " + control.OnChange + "();\n");
                        else
                            str.Append("\t\t\t\t" + control.OnChange + "();\n");
                    }
                    str.Append("}));\n");
                    ///Add Refrence to control
                    str.Append("\t\t\tbuilder.AddComponentReferenceCapture(1, txt =>\n");
                    str.Append("\t\t\t{\n");
                    if (control.ControlType == ControlType.String)
                        str.Append("\t\t\t\ttxt" + parameterName + " = txt as StringTextBox;\n");
                    else
                        str.Append("\t\t\t\ttxt" + parameterName + " = txt as NumericTextBox<" + strType + ">;\n");
                    str.Append("\t\t\t});\n");
                    //-----------------------------------------
                    str.Append("\t\t\tbuilder.CloseComponent();\n");
                    break;
                case ControlType.DropdownList:
                    strType = parameterName;
                    str.Append("\t\t\tbuilder.OpenComponent<DropdownList<" + strType + ">>(2);\n");
                    ///Value Binding
                    str.Append("\t\t\tbuilder.AddAttribute(3, \"Value\", " + parameterName + ");\n");
                    str.Append("\t\t\tbuilder.AddAttribute(3, \"ValueChanged\", EventCallback.Factory.Create<" + strType + ">(this," + (isAsync == true ? "async" : "") + " value => {\n" + parameterName + " = value;\n");
                    if (control.OnChange.HasValue())
                    {
                        if (isAsync == true)
                            str.Append("\t\t\t\tawait " + control.OnChange + "();\n");
                        else
                            str.Append("\t\t\t\t" + control.OnChange + "();\n");
                    }
                    str.Append("}));\n");
                    ///Add Refrence to control
                    str.Append("\t\t\tbuilder.AddComponentReferenceCapture(1, ddl =>\n");
                    str.Append("\t\t\t{\n");
                    str.Append("\t\t\t\tddl" + parameterName + " = ddl as DropdownList<" + strType + ">;\n");
                    str.Append("\t\t\t});\n");
                    str.Append("\t\t\tbuilder.CloseComponent();\n");
                    break;
                default:
                    throw new NotImplementedException("خطای عدم پیاده سازی");
            }
            str.Append("\t\t\tbuilder.CloseElement();\n");
        }

        async Task CreateControl(Caspian.Engine.BlazorControl component, StringBuilder str, string userCode)
        {
            if (component.DynamicParameterId.HasValue || component.CustomeFieldName.HasValue())
            {
                CreateParameterControl(component, str, userCode);
                return;
            } 
            bool? isAsync = null;
            if (component.OnChange.HasValue())
                isAsync = new CodeManager().MethodIsAsync(form.Name, component!.OnChange, userCode);
            str.Append("\t\t\tbuilder.OpenElement(1, \"fieldset\");\n");
            str.Append("\t\t\tbuilder.AddAttribute(3, \"class\", \"c-dynamic-form-controls\");\n");
            if (component.MultiLine && component.Height > 1)
            {
                var style = "height:" + ((component.Height - 1) * 80 + 61).ToString() + "px";
                str.Append("\t\t\tbuilder.AddAttribute(3, \"style\", \"" + style + "\");");
            }
            str.Append("\t\t\tbuilder.OpenElement(1, \"legend\");\n");
            str.Append("\t\t\tbuilder.AddContent(1, \"" + component.Caption + "\");\n");
            str.Append("\t\t\tbuilder.CloseElement();\n");
            var type = new AssemblyInfo().GetModelType(form.WorkflowGroup.SubSystemKind, component.DataModelField.EntityFullName);
            var info = type.GetProperty(component.PropertyName);
            type = info!.PropertyType;
            var strType = type.GetUnderlyingType().Name;
            if (type.IsNullableType())
                strType += "?";
            var scope = CreateScope();
            var service = new BlazorControlService(scope.ServiceProvider);
            var id = await service.GetId(form.WorkflowGroup.SubSystemKind, component);
            switch (component.ControlType)
            {
                case ControlType.String:
                    str.Append("\t\t\tbuilder.OpenComponent<StringTextBox>(2);\n");
                    if (component.MultiLine && component.Height > 1)
                    {
                        var style = "height:" + ((component.Height - 1) * 80 + 30).ToString() + "px";
                        str.Append("\t\t\tbuilder.AddAttribute(3, \"style\", \"" + style + "\");");
                        str.Append("\t\t\tbuilder.AddAttribute(3, \"MultiLine\", true);");
                    }
                    str.Append("\t\t\tbuilder.AddAttribute(3, \"Value\", " + component.DataModelField.FieldName + '.' + component.PropertyName + ");\n");
                    str.Append("\t\t\tbuilder.AddAttribute(3, \"ValueChanged\", EventCallback.Factory.Create<string>(this, value => { " + component.DataModelField.FieldName + '.' + component.PropertyName + " = value; }));\n");
                    str.Append("\t\t\tbuilder.CloseComponent();\n");
                    break;
                case ControlType.DropdownList:
                    str.Append("\t\t\tbuilder.OpenComponent<DropdownList<" + strType + ">>(2);\n");
                    str.Append("\t\t\tbuilder.AddAttribute(3, \"Value\", " + component.DataModelField.FieldName + '.' + component.PropertyName + ");\n");
                    str.Append("\t\t\tbuilder.AddAttribute(3, \"ValueChanged\", EventCallback.Factory.Create<" + strType + ">(this," + (isAsync == true ? "async" : "") + " value => \n\t\t\t{\n\t\t\t\t" + component.DataModelField.FieldName + '.' + component.PropertyName + " = value;\n");
                    if (component.OnChange.HasValue())
                    {
                        if (isAsync == true)
                            str.Append("\t\t\t\tawait " + component.OnChange + "();\n");
                        else
                            str.Append("\t\t\t\t" + component.OnChange + "();\n");
                    }
                        
                    str.Append("\t\t\t}));\n");
                    str.Append("\t\t\tbuilder.CloseComponent();\n");
                    break;
                case ControlType.Date:
                    str.Append("\t\t\tbuilder.OpenComponent<DatePicker<" + strType + ">>(2);\n");
                    str.Append("\t\t\tbuilder.AddAttribute(3, \"Value\", " + component.DataModelField.FieldName + '.' + component.PropertyName + ");\n");
                    str.Append("\t\t\tbuilder.AddAttribute(3, \"ValueChanged\", EventCallback.Factory.Create<" + strType + ">(this, value => { " + component.DataModelField.FieldName + '.' + component.PropertyName + " = value; }));\n");
                    str.Append("\t\t\tbuilder.CloseComponent();\n");
                    break;
                case ControlType.ComboBox:
                    var typeName = info.GetForeignKey().PropertyType.Name;
                    str.Append("\t\t\tbuilder.OpenComponent<ComboBox<" + typeName + ", " + strType + ">>(2);\n");
                    str.Append("\t\t\tbuilder.AddAttribute(3, \"Value\", " + component.DataModelField.FieldName + '.' + component.PropertyName + ");\n");
                    str.Append("\t\t\tbuilder.AddAttribute(3, \"ValueChanged\", EventCallback.Factory.Create<" + strType + ">(this, value => { " + component.DataModelField.FieldName + '.' + component.PropertyName + " = value; }));\n");
                    if (component.OnChange.HasValue())
                    {
                        if (isAsync == true)
                            str.Append("\t\t\tbuilder.AddAttribute(3, \"OnValueChanged\", EventCallback.Factory.Create(this, async () => await " + component.OnChange + "()));\n");
                        else
                            str.Append("\t\t\tbuilder.AddAttribute(3, \"OnValueChanged\", EventCallback.Factory.Create(this, () => " + component.OnChange + "()));\n");
                    }
                    str.Append("\t\t\tbuilder.AddComponentReferenceCapture(1, cmb =>\n");
                    str.Append("\t\t\t{\n");
                    str.Append("\t\t\t\t" + id + " = cmb as ComboBox<" + typeName + ", " + strType + ">;\n");
                    if (component.TextExpression.HasValue())
                        str.Append("\t\t\t\t" + id + ".TextExpression = " + component.TextExpression + ";\n");
                    if (component.FilterExpression.HasValue())
                        str.Append("\t\t\t\t" + id + ".ConditionExpression = " + component.FilterExpression + ";\n");
                    str.Append("\t\t\t});\n");
                    str.Append("\t\t\tbuilder.CloseComponent();\n");
                    break;
                case ControlType.Integer:
                case ControlType.Numeric:
                    strType = info.PropertyType.GetUnderlyingType().Name;
                    if (info.PropertyType.IsNullableType())
                        strType += "?";
                    str.Append("\t\t\tbuilder.OpenComponent<NumericTextBox<" + strType + ">>(2);\n");
                    str.Append("\t\t\tbuilder.AddAttribute(3, \"Value\", " + component.DataModelField.FieldName + '.' + component.PropertyName + ");\n");
                    str.Append("\t\t\tbuilder.AddAttribute(3, \"ValueChanged\", EventCallback.Factory.Create<" + strType + ">(this, value => { " + component.DataModelField.FieldName + '.' + component.PropertyName + " = value; }));\n");

                    str.Append("\t\t\tbuilder.AddComponentReferenceCapture(1, txt =>\n");
                    str.Append("\t\t\t{\n");
                    str.Append("\t\t\t\t" + id + " = txt as NumericTextBox<" + strType + ">;\n");
                    str.Append("\t\t\t});\n");

                    str.Append("\t\t\tbuilder.CloseComponent();\n");
                    break;
                default:
                    throw new NotImplementedException("خطای عدم پیاده سازی");
            }
            str.Append("\t\t\tbuilder.CloseElement();\n");
        }

        void CreateAssembly(string codeToCompile)
        {
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(codeToCompile);
            string assemblyName = Path.GetRandomFileName();
            var modelPath = new AssemblyInfo().RelatedPath + "\\" + form.WorkflowGroup.SubSystemKind.ToString() + ".model.dll";
            var servicePath = new AssemblyInfo().RelatedPath + "\\" + form.WorkflowGroup.SubSystemKind.ToString() + ".service.dll";
            var refPaths = new[] {
                    typeof(System.Object).GetTypeInfo().Assembly.Location,
                    typeof(PersianDate).GetTypeInfo().Assembly.Location,
                    typeof(ComponentBase).GetTypeInfo().Assembly.Location,
                    typeof(StringTextBox).GetTypeInfo().Assembly.Location,
                    typeof(IServiceScope).GetTypeInfo().Assembly.Location,
                    typeof(AbstractValidator<>).GetTypeInfo().Assembly.Location,
                    typeof(Caspian.Common.AssemblyInfo).GetTypeInfo().Assembly.Location,
                    typeof(System.Linq.Expressions.BinaryExpression).Assembly.Location,
                    typeof(Convert).GetTypeInfo().Assembly.Location,
                    modelPath,
                    servicePath,
                    Path.Combine(Path.GetDirectoryName(typeof(System.Runtime.GCSettings).GetTypeInfo().Assembly.Location), "System.Runtime.dll")
                };
            MetadataReference[] references = refPaths.Select(r => MetadataReference.CreateFromFile(r)).ToArray();
            CSharpCompilation compilation = CSharpCompilation.Create(assemblyName,
                syntaxTrees: new[] { syntaxTree },
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
            using var ms = new MemoryStream();
            EmitResult result = compilation.Emit(ms);

            if (!result.Success)
            {
                IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic =>
                    diagnostic.IsWarningAsError ||
                    diagnostic.Severity == DiagnosticSeverity.Error);

                foreach (Diagnostic diagnostic in failures)
                {
                    Console.Error.WriteLine("\t{0}: {1}", diagnostic.Id, diagnostic.GetMessage());
                }
            }
            else
            {
                ms.Seek(0, SeekOrigin.Begin);
                Assembly assembly = AssemblyLoadContext.Default.LoadFromStream(ms);
                DynamicType = assembly.GetType("Caspian.Engine.CodeGenerator." + form.Name);
            }
            isloading = false;
        }

        [Parameter]
        public int WorkflowFormId { get; set; }
    }
}
