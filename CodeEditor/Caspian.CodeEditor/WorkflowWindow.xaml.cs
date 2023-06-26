using System.IO;
using System.Windows;
using RoslynPad.Editor;
using RoslynPad.Roslyn;
using Microsoft.CodeAnalysis;
using System.Reflection;
using System.Text.Json;
using RoslynPadReplSample;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for WorkflowWindow.xaml
    /// </summary>
    public partial class WorkflowWindow : Window
    {
        RoslynHost _host;
        DocumentId _documentId;
        CodeManager _codeManager;
        public WorkflowWindow()
        {
            InitializeComponent();
            _codeManager = new CodeManager();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;

            _host = new MyRoslynHost(additionalAssemblies: new[]
            {
                Assembly.Load("RoslynPad.Roslyn.Windows"),
                Assembly.Load("RoslynPad.Editor.Windows"),
            }, RoslynHostReferences.NamespaceDefault.With(assemblyReferences: new[]
            {
                typeof(object).Assembly,
                typeof(System.Text.RegularExpressions.Regex).Assembly,
                typeof(System.Linq.Enumerable).Assembly,
                typeof(Employment.Model.City).Assembly,
                typeof(Employment.Service.EmploymentOrderTypeService).Assembly,
                typeof(Capian.Dynamicform.Component.InputControl).Assembly,
                typeof(System.Linq.Expressions.BinaryExpression).Assembly,
            }));
        }


        private async void tabControl_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (tabControl.SelectedIndex == 1)
            {
                await webViewForm.CoreWebView2.ExecuteScriptAsync("$.workflow.getActivityCodebehindString()");
            }
            else if (tabControl.SelectedIndex == 2)
            {
                if (codeEditor.Text == "")
                    await webViewForm.CoreWebView2.ExecuteScriptAsync("$.workflow.getActivitySourceCodeString()");
            }
        }

        private void webViewForm_WebMessageReceived(object sender, Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs e)
        {
            var data = JsonSerializer.Deserialize<JsonObject>(e.WebMessageAsJson)!;
            switch (data.Action)
            {
                case "setActivityCodebehind":
                    codeBehindEditor.Text = data.Content;
                    break;
                case "setActivitySourceCode":
                    codeEditor.Text = data.Content;
                    break;
            }
        }

        private void Editor_CreatingDocument(object sender, RoslynPad.Editor.CreatingDocumentEventArgs args)
        {
            var workingDirectory = Directory.GetCurrentDirectory();
            if (_documentId == null)
            {
                args.DocumentId = _host.AddDocument(new DocumentCreationArgs(
                    args.TextContainer, workingDirectory, args.ProcessDiagnostics,
                    args.TextContainer.UpdateText));
            }
            else
            {
                args.DocumentId = _host.AddRelatedDocument(_documentId, new DocumentCreationArgs(
                    args.TextContainer, workingDirectory, args.ProcessDiagnostics,
                    args.TextContainer.UpdateText));
            }
        }

        private void Editor_Loaded(object sender, RoutedEventArgs e)
        {
            var editor = (RoslynCodeEditor)sender;
            editor.Loaded -= Editor_Loaded;
            var workingDirectory = Directory.GetCurrentDirectory();
            _documentId = editor.Initialize(_host, new ClassificationHighlightColors(), workingDirectory, "");
        }
    }
}
