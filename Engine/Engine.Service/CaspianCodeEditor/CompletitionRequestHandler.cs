using Caspian.Common;

namespace Engine.Service.CaspianCodeEditor
{
    public static class CompletitionRequestHandler
    {
        public async static Task<TabCompletionResult[]> HandleCompletion(int position, string source, string codeBehind, string[] assemblies)
        {
            var workspace = CompletionWorkspace.Create(assemblies);
            var document = await workspace.CreateDocumentComplete(source, codeBehind);
            var list = (await document.GetTabCompletion(position, CancellationToken.None)).ToList();
            return list.ToArray();
        }

        public async static Task<HoverInfoResult> HandleHover(int position, string source, string codeBehind, string[] assemblies)
        {
            var workspace = CompletionWorkspace.Create(assemblies);
            var document = await workspace.CreateDocumentComplete(source, codeBehind);
            return await document.GetHoverInformation(position, CancellationToken.None);
        }

        public async static Task<CodeCheckResult[]> HandleCodeCheck(string source, string codeBehind, string[] assemblies)
        {
            var workspace = CompletionWorkspace.Create(assemblies);
            var document = await workspace.CreateDocumentCodeCheck(source, codeBehind);
            return await document.GetCodeCheckResults(CancellationToken.None);
        }

        //public async static Task<SignatureHelpResult> HandleSignature(int position, string source, string codeBehind, string[] assemblies)
        //{
        //    var workspace = CompletionWorkspace.Create(assemblies);
        //    var document = await workspace.CreateDocument(source, codeBehind);
        //    return await document.GetSignatureHelp(position, CancellationToken.None);
        //}
    }
}
