using System.Linq;
using RoslynPad.Roslyn;
using System.Reflection;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.Host;
using System.Collections.Immutable;

namespace WpfApp1
{
    public class MyRoslynWorkSpace: RoslynWorkspace
    {
        public MyRoslynWorkSpace(HostServices hostServices, string workspaceKind = "Host", RoslynHost? roslynHost = null) :
            base(hostServices, workspaceKind, roslynHost)
        {

        }

        protected override bool PartialSemanticsEnabled => true;
    }

    public class MyRoslynHost: RoslynHost
    {
        public MyRoslynHost(IEnumerable<Assembly>? additionalAssemblies = null, RoslynHostReferences? references = null, 
            ImmutableArray<string>? disabledDiagnostics = null)
            :base(additionalAssemblies, references, disabledDiagnostics)
        {

        }
        protected override ParseOptions CreateDefaultParseOptions()
        {
            var parent = base.CreateDefaultParseOptions();
            parent = parent.WithKind(SourceCodeKind.Regular);
            return parent;
        }
        MyRoslynWorkSpace worlkpace;
        public override RoslynWorkspace CreateWorkspace()
        {
            worlkpace = new MyRoslynWorkSpace(HostServices, "Host", this);
            return worlkpace;
        }

        protected override Project CreateProject(Solution solution, DocumentCreationArgs args, CompilationOptions compilationOptions, Project? previousProject = null)
        {
            if (worlkpace.CurrentSolution.Projects.Count() == 0)
                return base.CreateProject(worlkpace.CurrentSolution, args, compilationOptions, previousProject);
            return worlkpace.CurrentSolution.Projects.First();
        }
    }
}
