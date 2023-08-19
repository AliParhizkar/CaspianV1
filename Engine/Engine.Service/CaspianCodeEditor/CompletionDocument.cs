using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Emit;

namespace Engine.Service.CaspianCodeEditor
{
    public class CompletionDocument
    {
        private readonly Document sourceDocument;
        private readonly SemanticModel semanticModel;
        private readonly EmitResult emitResult;

        //private QuickInfoProvider quickInfoProvider;

        internal CompletionDocument(Document sourceDocument, SemanticModel semanticModel, EmitResult emitResult)
        {
            this.sourceDocument = sourceDocument;
            this.semanticModel = semanticModel;
            this.emitResult = emitResult;

            //this.quickInfoProvider = new QuickInfoProvider(new DeferredQuickInfoContentProvider());
        }

        public Task<HoverInfoResult> GetHoverInformation(int position, CancellationToken cancellationToken)
        {
            //var info = await quickInfoProvider.GetItemAsync(document, position, cancellationToken);
            //return new HoverInfoResult() { Information = info.Create().ToString() };
            var hoverInformationProvider = new HoverInformationProvider();
            return hoverInformationProvider.Provide(sourceDocument, position, semanticModel);
        }

        public async Task<TabCompletionResult[]> GetTabCompletion(int position, CancellationToken cancellationToken)
        {
            var tabCompletionProvider = new TabCompletionProvider();
            return await tabCompletionProvider.Provide(sourceDocument, position);
        }

        public async Task<CodeCheckResult[]> GetCodeCheckResults(CancellationToken cancellationToken)
        {
            var codeCheckProvider = new CodeCheckProvider();
            return await codeCheckProvider.Provide(emitResult, sourceDocument, cancellationToken);
        }

        public Task<SignatureHelpResult> GetSignatureHelp(int position, CancellationToken cancellationToken)
        {
            var signatureHelpProvider = new SignatureHelpProvider();
            return signatureHelpProvider.Provide(sourceDocument, position, semanticModel);
        }
    }
}
