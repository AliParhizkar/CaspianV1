using Caspian.UI;
using System.Text;
using Caspian.Common;
using FluentValidation;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.CodeAnalysis;
using Caspian.Common.Extension;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.DependencyInjection;

namespace Caspian.Engine.Service
{
    public static class WorkflowGroupExtension
    {
        public static string GetCode(this WorkflowGroup group, WorkflowForm form, string userCode)
        {
            var str = new StringBuilder();
            str.Append("using Caspian.UI;\n");
            str.Append("using System;\n");
            str.Append("using Caspian.Common.Attributes;\n");
            str.Append($"using {group.SubSystemKind}.Model;\n");
            str.Append($"using {group.SubSystemKind}.Service;\n");
            str.Append($"using {group.SubSystemKind}.Web.Pages;\n");
            str.Append("using Microsoft.AspNetCore.Components;\n");
            str.Append("using Caspian.Common;\n");
            str.Append("using System.Threading.Tasks;\n");
            str.Append("using System.ComponentModel.DataAnnotations;\n\n");
            str.Append("using Microsoft.AspNetCore.Components.Rendering;\n\n");
            str.Append("namespace Caspian.Engine.CodeGenerator\n{\n\t");
            str.Append(form.GetCode(userCode));
            if (userCode.HasValue())
                str.Append("\n\n" + userCode);
            str.Append("\n}");
            return str.ToString();
        }

        public static async Task<Type> GetFormType (this WorkflowForm form, string rootPath)
        {
            var userCode = await form.GetSourceFile(rootPath);
            userCode += form.CreateEnumesCode();
            var strSource = form.WorkflowGroup.GetCode(form, userCode);
            return form.WorkflowGroup.CreateAssembly(form.Name, strSource);
        }

        public static Type CreateAssembly(this WorkflowGroup group, string formName, string codeToCompile)
        {
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(codeToCompile);
            string assemblyName = Path.GetRandomFileName();
            var modelPath = new AssemblyInfo().RelatedPath + "\\" + group.SubSystemKind.ToString() + ".model.dll";
            var servicePath = new AssemblyInfo().RelatedPath + "\\" + group.SubSystemKind.ToString() + ".service.dll";
            var webPagePath = new AssemblyInfo().RelatedPath + "\\" + group.SubSystemKind.ToString() + ".Web.dll";
            var refPaths = new[] {
                    typeof(object).GetTypeInfo().Assembly.Location,
                    typeof(PersianDate).GetTypeInfo().Assembly.Location,
                    typeof(ComponentBase).GetTypeInfo().Assembly.Location,
                    typeof(StringTextBox).GetTypeInfo().Assembly.Location,
                    typeof(IServiceScope).GetTypeInfo().Assembly.Location,
                    typeof(AbstractValidator<>).GetTypeInfo().Assembly.Location,
                    typeof(DisplayAttribute).GetTypeInfo().Assembly.Location,
                    typeof(AssemblyInfo).GetTypeInfo().Assembly.Location,
                    typeof(System.Linq.Expressions.BinaryExpression).Assembly.Location,
                    typeof(Convert).GetTypeInfo().Assembly.Location,
                    modelPath,
                    servicePath,
                    webPagePath,
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
                throw new CaspianException(failures.First().GetMessage());
            }
            else
            {
                ms.Seek(0, SeekOrigin.Begin);
                Assembly assembly = AssemblyLoadContext.Default.LoadFromStream(ms);
                return assembly.GetType("Caspian.Engine.CodeGenerator." + formName);
            }
        }
    }
}
