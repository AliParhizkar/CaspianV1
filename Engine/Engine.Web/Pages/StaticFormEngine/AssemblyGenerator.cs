using Caspian.UI;
using System.Text;
using Caspian.Common;
using System.Reflection;
using System.Runtime.Loader;
using Caspian.Engine.Service;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.AspNetCore.Components;
using Caspian.Common.Extension;
using Microsoft.EntityFrameworkCore;
using FluentValidation;

namespace Caspian.Engine.WorkflowEngine
{
    public partial class AssemblyGenerator: BasePage 
    {
        WorkflowForm form;
        Type? DynamicType;
        bool isloading = false;

        protected override async Task OnInitializedAsync()
        {
            if (DynamicType == null)
            {
                if (!isloading)
                {
                    using var scope = CreateScope();
                    var service = new WorkflowFormService(scope);
                    form = await service.GetAll().Include("EntityFields").Include("Rows").Include("Rows.Columns")
                        .Include("Rows.Columns.Component").SingleAsync(t => t.Id == WorkflowFormId);
                    isloading = true;
                    var userSource = GetSourceFile();
                    var strSource = await CreateCodebehindFormFile(userSource);
                    CreateAssembly(strSource);
                }
            }
            await base.OnInitializedAsync();
        }

        void ForTest(StringBuilder str)
        {
            str.Append("\t\t\tbuilder.OpenElement(1, \"fieldset\");\n");
            str.Append("\t\t\tbuilder.OpenElement(1, \"legend\");\n");
            str.Append("\t\t\tbuilder.AddContent(1, \"مشخصات فیلدها\");\n");
            str.Append("\t\t\tbuilder.CloseElement();\n");
            str.Append("\t\t\tbuilder.OpenElement(1, \"span\");\n");
            str.Append("\t\t\tbuilder.AddContent(1, " + "childrenProperty.Gender" + ");\n");
            str.Append("\t\t\tbuilder.CloseElement();\n");
            str.Append("\t\t\tbuilder.CloseElement();\n");
        }

        string? GetSourceFile()
        {
            if (form.SourceFileName.HasValue())
            {
                var path = Environment.ContentRootPath + "Data\\Code\\" + form.SourceFileName + ".cs";
                var content =  File.ReadAllText(path);
                return new CodeManager().FindSourceCode(content);
            }
            return null;
        }

        async Task<string> CreateCodebehindFormFile(string userCode)
        {
            using var scope = CreateScope();
            var service = new WorkflowFormService(scope);
            var str = new StringBuilder();
            str.Append("using Caspian.UI;\n");
            str.Append("using System;\n");
            str.Append("using " + form.SubSystemKind.ToString() + ".Model;\n");
            str.Append("using " + form.SubSystemKind.ToString() + ".Service;\n");
            str.Append("using Microsoft.AspNetCore.Components;\n");
            str.Append("using Caspian.Common;\n");
            str.Append("using System.Threading.Tasks;\n");
            str.Append("using Microsoft.AspNetCore.Components.Rendering;\n\n");
            str.Append("namespace Caspian.Engine.CodeGenerator\n{\n\t");
            str.Append("public partial class " + form.Name + ": BasePage\n");
            str.Append("\t{\n");
            foreach (var field in form.EntityFields)
                str.Append("\t\t" + field.EntityFullName + ' ' + field.FieldName + ";\n");
            foreach (var row in form.Rows)
                foreach(var col in row.Columns)
                {
                    var ctr = col.Component;
                    if (ctr != null)
                        str.Append(service.GetControlType(form.SubSystemKind, ctr, true));
                }
            str.Append("\t\tprotected override void OnInitialized()\n\t\t{\n");
            foreach(var field in form.EntityFields)
                str.Append("\t\t\t" + field.FieldName + " = new " + field.EntityFullName + "();\n"); ;
            str.Append("\t\t\tbase.OnInitialized();\n");
            str.Append("\t\t}\n\n");
            str.Append("\t\tprotected override void BuildRenderTree(RenderTreeBuilder builder)\n\t\t{\n");
            //Create MessageBox Component
            str.Append("\t\t\tbuilder.OpenComponent<MessageBox>(2);");
            str.Append("\t\t\tbuilder.AddComponentReferenceCapture(1, msg => { MessageBox = msg as MessageBox; });");
            str.Append("\t\t\tbuilder.CloseComponent();");
            foreach (var row in form.Rows)
            {
                var factor = 12 / form.ColumnCount;
                str.Append("\t\t\tbuilder.OpenElement(1, \"div\");\n");
                str.Append("\t\t\tbuilder.AddAttribute(3, \"class\", \"row\");\n");
                foreach (var col in row.Columns)
                {
                    str.Append("\t\t\tbuilder.OpenElement(1, \"div\");\n");
                    str.Append("\t\t\tbuilder.AddAttribute(3, \"class\", \"col-md-" + factor + "\");\n");
                    if (col.Component != null)
                        await CreateControl(col.Component, str);
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

        async Task CreateControl(Caspian.Engine.BlazorControl component, StringBuilder str)
        {
            str.Append("\t\t\tbuilder.OpenElement(1, \"fieldset\");\n");
            str.Append("\t\t\tbuilder.AddAttribute(3, \"class\", \"c-dynamic-form-controls\");\n");
            str.Append("\t\t\tbuilder.OpenElement(1, \"legend\");\n");
            str.Append("\t\t\tbuilder.AddContent(1, \"" + component.Caption + "\");\n");
            str.Append("\t\t\tbuilder.CloseElement();\n");
            var type = new AssemblyInfo().GetModelType(form.SubSystemKind, component.WfFormEntityField.EntityFullName);
            var info = type.GetProperty(component.PropertyName);
            type = info!.PropertyType;
            var strType = type.GetUnderlyingType().Name;
            if (type.IsNullableType())
                strType += "?";
            var scope = CreateScope();
            var service = new BlazorControlService(scope);
            var id = await service.GetId(form.SubSystemKind, component);
            switch (component.ControlType)
            {
                case ControlType.String:
                    str.Append("\t\t\tbuilder.OpenComponent<StringTextBox>(2);\n");
                    str.Append("\t\t\tbuilder.AddAttribute(3, \"Value\", " + component.WfFormEntityField.FieldName + '.' + component.PropertyName + ");\n");
                    str.Append("\t\t\tbuilder.AddAttribute(3, \"ValueChanged\", EventCallback.Factory.Create<string>(this, value => { " + component.WfFormEntityField.FieldName + '.' + component.PropertyName + " = value; }));\n");
                    str.Append("\t\t\tbuilder.CloseComponent();\n");
                    break;
                case ControlType.DropdownList:
                    str.Append("\t\t\tbuilder.OpenComponent<DropdownList<" + strType + ">>(2);\n");
                    str.Append("\t\t\tbuilder.AddAttribute(3, \"Value\", " + component.WfFormEntityField.FieldName + '.' + component.PropertyName + ");\n");
                    str.Append("\t\t\tbuilder.AddAttribute(3, \"ValueChanged\", EventCallback.Factory.Create<" + strType + ">(this, value => \n\t\t\t{\n\t\t\t\t" + component.WfFormEntityField.FieldName + '.' + component.PropertyName + " = value;\n");
                    if (component.OnChange.HasValue())
                        str.Append("\t\t\t\tawait " + component.OnChange + "();\n");
                    str.Append("\t\t\t}));\n");
                    str.Append("\t\t\tbuilder.CloseComponent();\n");
                    break;
                case ControlType.Date:
                    str.Append("\t\t\tbuilder.OpenComponent<DatePicker<" + strType + ">>(2);\n");
                    str.Append("\t\t\tbuilder.AddAttribute(3, \"Value\", " + component.WfFormEntityField.FieldName + '.' + component.PropertyName + ");\n");
                    str.Append("\t\t\tbuilder.AddAttribute(3, \"ValueChanged\", EventCallback.Factory.Create<" + strType + ">(this, value => { " + component.WfFormEntityField.FieldName + '.' + component.PropertyName + " = value; }));\n");
                    str.Append("\t\t\tbuilder.CloseComponent();\n");
                    break;
                case ControlType.ComboBox:
                    var typeName = info.GetForeignKey().PropertyType.Name;
                    str.Append("\t\t\tbuilder.OpenComponent<ComboBox<" + typeName + ", " + strType + ">>(2);\n");
                    str.Append("\t\t\tbuilder.AddAttribute(3, \"Value\", " + component.WfFormEntityField.FieldName + '.' + component.PropertyName + ");\n");
                    str.Append("\t\t\tbuilder.AddAttribute(3, \"ValueChanged\", EventCallback.Factory.Create<" + strType + ">(this, value => { " + component.WfFormEntityField.FieldName + '.' + component.PropertyName + " = value; }));\n");
                    if (component.OnChange.HasValue())
                        str.Append("\t\t\tbuilder.AddAttribute(3, \"OnValueChanged\", EventCallback.Factory.Create(this, async () => await " + component.OnChange + "()));\n");
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
                    str.Append("\t\t\tbuilder.AddAttribute(3, \"Value\", " + component.WfFormEntityField.FieldName + '.' + component.PropertyName + ");\n");
                    str.Append("\t\t\tbuilder.AddAttribute(3, \"ValueChanged\", EventCallback.Factory.Create<" + strType + ">(this, value => { " + component.WfFormEntityField.FieldName + '.' + component.PropertyName + " = value; }));\n");

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
            var modelPath = new AssemblyInfo().RelatedPath + "\\" + form.SubSystemKind.ToString() + ".model.dll";
            var servicePath = new AssemblyInfo().RelatedPath + "\\" + form.SubSystemKind.ToString() + ".service.dll";
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
