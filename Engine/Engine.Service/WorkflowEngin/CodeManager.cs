using Caspian.Common;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Caspian.Engine.Service
{
    public class CodeManager
    {
        public ClassDeclarationSyntax GetClassOfForm(string className, string sourceCode)
        {
            var syntax = CSharpSyntaxTree.ParseText(sourceCode).GetRoot();
            if (syntax.IsKind(SyntaxKind.CompilationUnit))
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
                    else if (member.Kind() == SyntaxKind.ClassDeclaration)
                    {
                        var class_ = member as ClassDeclarationSyntax;
                        if (class_.Identifier.Text.Equals(className))
                            return class_;
                    }
                }
            }
            return null;
        }

        public MethodDeclarationSyntax GetInitializeMethod(string className, string sourceCode)
        {
            var classOfForm = GetClassOfForm(className, sourceCode);
            foreach (var member3 in classOfForm.Members)
            {
                if (member3.Kind() == SyntaxKind.MethodDeclaration)
                {
                    var method = member3 as MethodDeclarationSyntax;
                    if (method!.Identifier.Text == "Initialize")
                        return method;
                }
            }
            return null;
        }

        public string FindSourceCode(string sourceCode)
        {
            var syntax = CSharpSyntaxTree.ParseText(sourceCode).GetRoot();
            if (syntax.IsKind(SyntaxKind.CompilationUnit))
            {
                var span = ((syntax as CompilationUnitSyntax).Members.Single() as NamespaceDeclarationSyntax)
                    .Members.FullSpan;
                return sourceCode.Substring(span.Start, span.Length);
            }
            throw new NotImplementedException("خطای عدم پیاده ساری");
        }

        public bool MethodIsAsync(string className, string methodName, string sourceCode)
        {
            var method = FindMethod(className, methodName, sourceCode);
            if (method == null)
                throw new CaspianException($"خطا: Method with name {methodName} in type {className} not exist");
            CSharpSyntaxNode type = method.ReturnType;
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

        public bool MethodIsExist(string className, string methodName, string sourceCode)
        {
            return FindMethod(className, methodName, sourceCode) != null;
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
    }
}
