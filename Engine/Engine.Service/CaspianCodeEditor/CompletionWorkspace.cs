using System.Xml;
using System.Data;
using System.Reflection;
using System.ComponentModel;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Host.Mef;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Engine.Service.CaspianCodeEditor
{
    public class CompletionWorkspace
    {
        public static MetadataReference[] DefaultMetadataReferences = new MetadataReference[]
            {
                MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),
                MetadataReference.CreateFromFile(Assembly.Load("System.Runtime").Location),
                MetadataReference.CreateFromFile(typeof(List<>).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(int).Assembly.Location),
                MetadataReference.CreateFromFile(Assembly.Load("netstandard").Location),
                MetadataReference.CreateFromFile(typeof(DescriptionAttribute).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(DataSet).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(XmlDocument).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(DisplayAttribute).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(INotifyPropertyChanged).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(System.Linq.Expressions.Expression).Assembly.Location)
            };
        private Project _project;
        private AdhocWorkspace _workspace;
        private List<MetadataReference> _metadataReferences;

        public static CompletionWorkspace Create(params string[] assemblies)
        {
            Assembly[] lst = new[] {
                Assembly.Load("Microsoft.CodeAnalysis.Workspaces"),
                Assembly.Load("Microsoft.CodeAnalysis.CSharp.Workspaces"),
                Assembly.Load("Microsoft.CodeAnalysis.Features"),
                Assembly.Load("Microsoft.CodeAnalysis.CSharp.Features")
            };

            var host = MefHostServices.Create(lst);
            var workspace = new AdhocWorkspace(host);

            var references = DefaultMetadataReferences.ToList();

            if (assemblies != null && assemblies.Length > 0)
            {
                for (int i = 0; i < assemblies.Length; i++)
                {
                    var path = Assembly.Load(assemblies[i]).Location;
                    references.Add(MetadataReference.CreateFromFile(path));
                }
            }

            var projectInfo = ProjectInfo.Create(ProjectId.CreateNewId(), VersionStamp.Create(), "TempProject", "TempProject", LanguageNames.CSharp)
                .WithMetadataReferences(references);
            var project = workspace.AddProject(projectInfo);


            return new CompletionWorkspace() { _workspace = workspace, _project = project, _metadataReferences = references };
        }

        public async Task<CompletionDocument> CreateDocumentCodeCheck(string source, string codeBehind)
        {
            var codeBehindDocument = _workspace.AddDocument(_project.Id, "Codebehind.cs", SourceText.From(codeBehind));
            var sourceDocument = _workspace.AddDocument(_project.Id, "SourceFile.cs", SourceText.From(source));
            var codeBehindSyntaxTree = await codeBehindDocument.GetSyntaxTreeAsync();
            var sourceSyntaxTree = await sourceDocument.GetSyntaxTreeAsync();
            var compilation = CSharpCompilation.Create("Temp", new[] { codeBehindSyntaxTree, sourceSyntaxTree },
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary),
                references: _metadataReferences);
            using var temp = new MemoryStream();
            var result = compilation.Emit(temp);
            var semanticModel = compilation.GetSemanticModel(codeBehindSyntaxTree, true);

            return new CompletionDocument(sourceDocument, semanticModel, result);
        }

        public CompletionDocument CreateDocumentComplete(string source, string codeBehind)
        {
            _workspace.AddDocument(_project.Id, "Codebehind.cs", SourceText.From(codeBehind));//SourceFile
            var document = _workspace.AddDocument(_project.Id, "SourceFile.cs", SourceText.From(source));
            _workspace.TryApplyChanges(_workspace.CurrentSolution);
            return new CompletionDocument(document, null, null);
        }
    }
}
