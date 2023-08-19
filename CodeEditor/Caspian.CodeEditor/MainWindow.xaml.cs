using System;
using WpfApp1;
using System.IO;
using System.Linq;
using System.Windows;
using RoslynPad.Editor;
using RoslynPad.Roslyn;
using System.Text.Json;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Text.Json.Serialization;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoslynPadReplSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        RoslynHost _host;
        DocumentId _documentId;
        CodeManager _codeManager;

        public MainWindow()
        {
            InitializeComponent();
            _codeManager = new CodeManager();
            Loaded += OnLoaded;
            //var result = new CodeManager().MethodIsAsync("EmploymentOrderPage", "cmbEmploymentOrderTypeOnChange", File.ReadAllText("C:\\Users\\Ali\\source\\repos\\Caspian\\UIComponent\\List\\ComboBox.cs"));
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;
            _host = new MyRoslynHost(additionalAssemblies: new[]
            {
                Assembly.Load("RoslynPad.Roslyn.Windows"),
                Assembly.Load("RoslynPad.Editor.Windows"),
            }, RoslynHostReferences.NamespaceDefault.With(assemblyReferences:new[]
            {
                typeof(object).Assembly,
                typeof(System.Text.RegularExpressions.Regex).Assembly,
                typeof(System.Linq.Enumerable).Assembly,
                typeof(Employment.Model.City).Assembly,
                typeof(Employment.Service.EmploymentOrderTypeService).Assembly,
                typeof(Capian.Dynamicform.Component.InputControl).Assembly,
                typeof(System.Linq.Expressions.BinaryExpression).Assembly,
                typeof(System.ComponentModel.DataAnnotations.DisplayAttribute).Assembly,
            }));
        }

        private void Editor_Loaded(object sender, RoutedEventArgs e)
        {
            var editor = (RoslynCodeEditor)sender;
            editor.Loaded -= Editor_Loaded;
            var workingDirectory = Directory.GetCurrentDirectory();
            _documentId = editor.Initialize(_host, new ClassificationHighlightColors(), workingDirectory, "");
        }

        private void Editor_CreatingDocument(object sender, CreatingDocumentEventArgs args)
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

        void SelectNode(CSharpSyntaxNode node, string txt = null)
        {
            tabControl.SelectedIndex = 2;
            var q = node.Kind();
            switch (node.Kind())
            {
                case SyntaxKind.Block:
                    var close = (node as BlockSyntax)!.CloseBraceToken;
                    var line = node.SyntaxTree.GetLineSpan(close.Span).StartLinePosition.Line;
                    var lineSpan = node.SyntaxTree.GetText().Lines[line - 1].Span;
                    var result = node.SyntaxTree.GetRoot().DescendantNodes(lineSpan).FirstOrDefault(n => lineSpan.Contains(n.Span));
                    if (result == null)
                    {
                        codeEditor.Text = codeEditor.Text.Insert(lineSpan.Start, "\t\t\t" + txt);
                        codeEditor.Select(lineSpan.End, 1);
                        tabControl.SelectedIndex = 2;
                    }
                    else
                    {
                        codeEditor.Text = codeEditor.Text.Insert(lineSpan.End, "\n\t\t\t" + txt);
                        //tabControl.SelectedIndex = 2;;
                    }
                    break;
            }
            //codeEditor.SelectionStart = node.Right.SpanStart;
            //codeEditor.SelectionLength = node.Right.Span.Length;
        }

        public void AnalizeNamespce(SyntaxList<MemberDeclarationSyntax> namespaces)
        {
            foreach(NamespaceDeclarationSyntax namespace_ in namespaces)
            {
                var name = namespace_.Name.ToString();
                foreach(ClassDeclarationSyntax class_ in namespace_.Members.Where(t => t.Kind() == SyntaxKind.ClassDeclaration))
                {
                    var className = class_.Identifier.Text;
                    foreach(MethodDeclarationSyntax func in class_.Members.Where(t => t.Kind() == SyntaxKind.MethodDeclaration))
                    {
                        var memberName = func.Identifier.Text;
                        var nodes = func.ChildNodes();
                        foreach (BlockSyntax node in nodes.Where(t => t.Kind() == SyntaxKind.Block))
                        {
                            foreach(ExpressionStatementSyntax statement in node.Statements.Where(t => t.Kind() == SyntaxKind.ExpressionStatement))
                            {
                                if (statement.Expression.Kind() == SyntaxKind.SimpleAssignmentExpression)
                                {
                                    var expr = statement.Expression as AssignmentExpressionSyntax;
                                    if (expr.Left.Kind() == SyntaxKind.SimpleMemberAccessExpression)
                                    {
                                        var expr1 = expr.Left as MemberAccessExpressionSyntax;
                                        if (expr1.Name.Identifier.ToString() == "FilterExpression")
                                        {
                                            var line = expr.SyntaxTree.GetLineSpan(expr.Span).StartLinePosition.Line + 1;
                                            tabControl.SelectedIndex = 2;
                                            codeEditor.SelectionStart = expr.Right.SpanStart;
                                            codeEditor.SelectionLength = expr.Right.Span.Length;
                                            var expr2 = expr1.Expression as IdentifierNameSyntax;
                                            var name123 = expr2.Identifier.ToString();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        
        private async void tabControl_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }

        private async void webViewForm_WebMessageReceived(object sender, Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs e)
        {
            var data = JsonSerializer.Deserialize<JsonObject>(e.WebMessageAsJson)!;
            switch(data.Action)
            {
                case "findCode":
                    tabControl.Visibility = Visibility.Visible;
                    tabControl.SelectedIndex = 2;
                    var dynamicObject = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(data.Content);
                    var className = dynamicObject.ClassName.Value;
                    var id = dynamicObject.Id.Value;
                    var Property = dynamicObject.Property.Value;
                    if (codeEditor.Text != "")
                    {
                        var methodSyntax = _codeManager.GetInitializeMethod(className, codeEditor.Text);
                        
                    }
                    break;
                case "findEventHandler":
                    tabControl.SelectedIndex = 2;
                    dynamicObject = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(data.Content);
                    className = dynamicObject.ClassName.Value;
                    id = dynamicObject.Id.Value;
                    var eventHandler = dynamicObject.EventHandler.Value;
                    if (codeEditor.Text == "")
                        await webViewForm.CoreWebView2.ExecuteScriptAsync("$.workflowForm.getSourceCodeString()");
                    if (codeEditor.Text != "")
                    {
                        ClassDeclarationSyntax mainClass = _codeManager.GetClassOfForm(className, codeEditor.Text);
                        MethodDeclarationSyntax methodSyntax = _codeManager.GetEventHandler(mainClass, eventHandler);
                        if (methodSyntax == null)
                        {
                            var close = mainClass.GetLastToken();
                            var line = mainClass.SyntaxTree.GetLineSpan(close.FullSpan).StartLinePosition.Line;
                            var lineSpan = mainClass.SyntaxTree.GetText().Lines[line].Span;
                            var strStart = codeEditor.Text.Substring(0, lineSpan.Start);
                            var strMethod = "\n\t\tpublic void " + eventHandler + "()\n\t\t{\n\t\t\t\n\t\t}\n";
                            var strEnd = codeEditor.Text.Substring(lineSpan.Start);
                            codeEditor.Text = strStart + strMethod + strEnd;
                            codeEditor.Select(strStart.Length + strMethod.Length - 5, 1);
                        }
                        else
                        {
                            var close = methodSyntax.GetLastToken();
                            var line = methodSyntax.SyntaxTree.GetLineSpan(close.Span).StartLinePosition.Line;
                            var lineSpan = methodSyntax.SyntaxTree.GetText().Lines[line - 1].Span;
                            var result = methodSyntax.SyntaxTree.GetRoot().DescendantNodes(lineSpan).FirstOrDefault(n => lineSpan.Contains(n.Span));
                            if (result == null)
                            {
                                codeEditor.Select(lineSpan.End, 1);
                                
                            }
                        }
                    }
                    break;
            }
        }
    }

    public class JsonObject
    {
        [JsonPropertyName("action")]
        public string Action { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }
    }
}
