using System;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace WpfApp1
{
    internal class CodeManager
    {
        public ClassDeclarationSyntax GetClassOfForm(string className, string sourceCode)
        {
            var syntax = CSharpSyntaxTree.ParseText(sourceCode).GetRoot();
            if (syntax.Kind() == SyntaxKind.CompilationUnit)
            {
                var members = (syntax as CompilationUnitSyntax).Members;
                foreach (var member in members)
                {
                    if (member.Kind() == SyntaxKind.NamespaceDeclaration)
                    {
                        var nameSpace = member as NamespaceDeclarationSyntax;
                        if (nameSpace.Name.ToString() == "Caspian.Dynamic.WorkflowForm")
                        {
                            foreach (var member2 in nameSpace.Members)
                            {
                                if (member2.Kind() == SyntaxKind.ClassDeclaration)
                                {
                                    var class_ = member2 as ClassDeclarationSyntax;
                                    if (class_.Identifier.Text == className)
                                        return class_;
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }

        public MethodDeclarationSyntax GetEventHandler(ClassDeclarationSyntax classOfForm, string eventHandler)
        {
            foreach (var member3 in classOfForm.Members)
            {
                if (member3.Kind() == SyntaxKind.MethodDeclaration)
                {
                    var method = member3 as MethodDeclarationSyntax;
                    if (method!.Identifier.Text == eventHandler)
                        return method;
                }
            }
            return null;
        }

        public MethodDeclarationSyntax GetInitializeMethod(string className, string sourceCode)
        {
            return FindMethod(className, "Initialize", sourceCode);
        }

        public bool MethodIsAsync(string className, string methodName, string sourceCode)
        {
            CSharpSyntaxNode type = FindMethod(className, methodName, sourceCode).ReturnType;
            if (type.Kind() == SyntaxKind.IdentifierName)
            {
                return (type as IdentifierNameSyntax).Identifier.Text == "Task";
            } 
            if (type.Kind() == SyntaxKind.GenericName)
            {
                return (type as GenericNameSyntax).Identifier.Text == "Task";
            }
            return false;
        }

        public MethodDeclarationSyntax FindMethod(string className, string methodName, string sourceCode)
        {
            var classOfForm = GetClassOfForm(className, sourceCode);
            foreach (var member3 in classOfForm.Members)
            {
                if (member3.Kind() == SyntaxKind.MethodDeclaration)
                {
                    var method = member3 as MethodDeclarationSyntax;
                    //method.ReturnType.
                    if (method!.Identifier.Text == methodName)
                        return method;
                }
            }
            return null;
        }

        public CSharpSyntaxNode FindExpression(MethodDeclarationSyntax func, string property)
        {
            foreach (BlockSyntax node in func.ChildNodes().Where(t => t.Kind() == SyntaxKind.Block))
            {
                foreach (ExpressionStatementSyntax statement in node.Statements.Where(t => t.Kind() == SyntaxKind.ExpressionStatement))
                {
                    if (statement.Expression.Kind() == SyntaxKind.SimpleAssignmentExpression)
                    {
                        var expr = statement.Expression as AssignmentExpressionSyntax;
                        if (expr.Left.Kind() == SyntaxKind.SimpleMemberAccessExpression)
                        {
                            var expr1 = expr.Left as MemberAccessExpressionSyntax;
                            if (expr1.Name.Identifier.ToString() == property)
                            {
                                return expr1;
                                //var line = expr.SyntaxTree.GetLineSpan(expr.Span).StartLinePosition.Line + 1;
                                //tabControl.SelectedIndex = 2;
                                //codeEditor.SelectionStart = expr.Right.SpanStart;
                                //codeEditor.SelectionLength = expr.Right.Span.Length;
                                //var expr2 = expr1.Expression as IdentifierNameSyntax;
                                //var name123 = expr2.Identifier.ToString();
                            }
                        }
                    }
                }
                return node;
            }
            throw new NotImplementedException("خطای عدم پیاده سازی");
        }
    }
}
