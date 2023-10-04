using Caspian.Common;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Elfie.Model;

namespace Engine.Service.CodeTokenizer
{
    public class Tokenizer
    {
        IList<TokenData> Tokens;
        int[] linesLength;
        public IList<TokenData> GetTokenizeData(string source)
        {
            Tokens = new List<TokenData>();
            var root = CSharpSyntaxTree.ParseText(source).GetRoot();
            var tokens = root.ChildNodesAndTokens();
            linesLength = root.GetText().Lines.Select(t => t.Span.Length).ToArray();
            foreach (var token in tokens)
                Tokenize(token, source);
            Tokens = Tokens.OrderBy(t => t.Line).ThenBy(t => t.StartIndex).ToList();
            UpdateTokens();
            return Tokens;
        }

        void UpdateTokens()
        {
            var list = new List<TokenData>();
            foreach(var line in Tokens.Select(t => t.Line).Distinct())
            {
                var index = 0;
                foreach (var token in Tokens.Where(t => t.Line == line).OrderBy(t => t.StartIndex))
                {
                    if (index != token.StartIndex)
                        list.Add(new TokenData(line, index, token.StartIndex - index));
                    index = token.StartIndex + token.Length;
                    list.Add(token);
                }
                if (index < linesLength[line])
                    list.Add(new TokenData(line, index, linesLength[line] - index));
            }
            Tokens = list;
        }

        void Tokenize(SyntaxNodeOrToken node, string source)
        {
            var docs = node.GetLeadingTrivia().Where(t => t.IsDocument());
            foreach (var doc in docs)
                Tokens.AddRange(doc.AddCommentData(linesLength).ToArray());
            docs = node.GetTrailingTrivia().Where(t => t.IsDocument());
            foreach (var doc in docs)
                Tokens.AddRange(doc.AddCommentData(linesLength).ToArray());
            var kind = node.Kind();
            if (kind.ToString().EndsWith("Keyword"))
            {
                Tokens.Add(node.GetNodeData(TokenKind.Keyword));
            }
            else
            {
                switch (kind)
                {
                    ///Some of keyword
                    case SyntaxKind.SwitchKeyword:
                    case SyntaxKind.CaseKeyword:
                    case SyntaxKind.BreakKeyword:
                    case SyntaxKind.IfKeyword:
                    case SyntaxKind.ForKeyword:
                    case SyntaxKind.ForEachKeyword:
                    case SyntaxKind.WhileKeyword:
                    case SyntaxKind.InKeyword:
                    case SyntaxKind.DoKeyword:
                    case SyntaxKind.ReturnKeyword:
                        break;
                    case SyntaxKind.StringLiteralExpression:
                        //String 
                        break;
                    case SyntaxKind.IdentifierToken:
                        if (node.Parent.IsKindOf(SyntaxKind.ClassDeclaration, SyntaxKind.EnumDeclaration, SyntaxKind.StructDeclaration, 
                            SyntaxKind.InterfaceDeclaration))
                        {
                            Tokens.Add(node.GetNodeData(TokenKind.Type));
                        }
                        else if (node.Parent.IsKindOf(SyntaxKind.GenericName))
                        {
                            var temp = node.Parent;
                            while (temp != null && !temp.IsKindOf(SyntaxKind.InvocationExpression, SyntaxKind.VariableDeclaration, SyntaxKind.Parameter, SyntaxKind.PropertyDeclaration))
                                temp = temp.Parent;
                            if (temp != null)
                            {
                                if (temp.IsKindOf(SyntaxKind.InvocationExpression))
                                    Tokens.Add(node.GetNodeData(TokenKind.Method));
                                else
                                    Tokens.Add(node.GetNodeData(TokenKind.Type));
                            }
                        }
                        else if (node.Parent.IsKind(SyntaxKind.MethodDeclaration))
                        {
                            Tokens.Add(node.GetNodeData(TokenKind.Method));
                        }
                        else if (node.Parent.IsKind(SyntaxKind.Parameter))
                        {
                            Tokens.Add(node.GetNodeData(TokenKind.Identifire));
                        }
                        else if (node.Parent.IsKind(SyntaxKind.VariableDeclarator))
                        {
                            var parent = node.Parent;
                            while (parent.IsKindOf(SyntaxKind.VariableDeclarator, SyntaxKind.VariableDeclaration))
                                parent = parent.Parent;
                            if (parent.IsKind(SyntaxKind.LocalDeclarationStatement))
                            {
                                Tokens.Add(node.GetNodeData(TokenKind.Identifire));
                            }
                        }
                        else if (node.Parent.IsKindOf(SyntaxKind.IdentifierName, SyntaxKind.VariableDeclaration))
                        {
                            var identifire = node.Parent as IdentifierNameSyntax;
                            if (identifire.IsVar)
                            {
                                Tokens.Add(node.GetNodeData(TokenKind.Keyword));
                            }
                            else
                            {
                                var parent = identifire.Parent;
                                while (parent.IsKind(SyntaxKind.QualifiedName))
                                    parent = parent.Parent;
                                var isType = parent.IsKindOf(SyntaxKind.VariableDeclaration, SyntaxKind.PropertyDeclaration, SyntaxKind.Parameter, SyntaxKind.ObjectCreationExpression, SyntaxKind.SimpleBaseType);
                                if (isType)
                                {
                                    var qualifiedNameSyntax = (parent as VariableDeclarationSyntax)?.Type ?? (parent as ParameterSyntax)?.Type ?? (parent as ObjectCreationExpressionSyntax)?.Type ??
                                        (parent as PropertyDeclarationSyntax)?.Type ?? (parent as SimpleBaseTypeSyntax)?.Type;
                                    if (qualifiedNameSyntax.IsKind(SyntaxKind.IdentifierName))
                                        Tokens.Add(node.GetNodeData(TokenKind.Type));
                                    else
                                    {
                                        if ((qualifiedNameSyntax as QualifiedNameSyntax)?.Right == identifire)
                                            Tokens.Add(node.GetNodeData(TokenKind.Type));
                                    }

                                }
                            }
                        }
                        break;
                    case SyntaxKind.IdentifierName:
                        var tempNode = node.Parent;
                        while(tempNode.IsKind(SyntaxKind.SimpleMemberAccessExpression))
                            tempNode = tempNode.Parent;
                        if (node.Parent.IsKindOf(SyntaxKind.TypeOfExpression, SyntaxKind.TypeArgumentList))
                        {
                            Tokens.Add(node.GetNodeData(TokenKind.Type));
                        }
                        if (tempNode.IsKindOf(SyntaxKind.InvocationExpression))
                        {
                            var children = tempNode.ChildNodesAndTokens()[0].ChildNodesAndTokens();
                            if (children.Count == 1 && children[0].IsKind(SyntaxKind.IdentifierToken))
                                Tokens.Add(node.GetNodeData(TokenKind.Method));
                            if (children.Count == 3 && node == children[2])
                                Tokens.Add(node.GetNodeData(TokenKind.Method));
                        }
                        break;
                    case SyntaxKind.GenericName:
                    case SyntaxKind.RangeExpression:
                    case SyntaxKind.BaseExpression:
                    case SyntaxKind.BracketedArgumentList:
                    case SyntaxKind.LocalFunctionStatement:
                    case SyntaxKind.GreaterThanToken:
                    case SyntaxKind.OmittedTypeArgument:
                    case SyntaxKind.OmittedTypeArgumentToken:
                    case SyntaxKind.TypeArgumentList:
                    case SyntaxKind.Argument:
                    case SyntaxKind.InterfaceDeclaration:
                    case SyntaxKind.EmptyStatement:
                    case SyntaxKind.NullLiteralExpression:
                    case SyntaxKind.IncompleteMember:
                    case SyntaxKind.AccessorList:
                    case SyntaxKind.StructDeclaration:
                    case SyntaxKind.AndAssignmentExpression:
                    case SyntaxKind.AddAssignmentExpression:
                    case SyntaxKind.ColonToken:
                    case SyntaxKind.DoStatement:
                    case SyntaxKind.EnumDeclaration:
                    case SyntaxKind.EnumMemberDeclaration:
                    case SyntaxKind.NamespaceDeclaration:
                    case SyntaxKind.ForEachStatement:
                    case SyntaxKind.ArgumentList:
                    case SyntaxKind.ForStatement:
                    case SyntaxKind.GetAccessorDeclaration:
                    case SyntaxKind.LessThanExpression:
                    case SyntaxKind.LessThanToken:
                    case SyntaxKind.PlusPlusToken:
                    case SyntaxKind.PlusEqualsToken:
                    case SyntaxKind.PostIncrementExpression:
                    case SyntaxKind.QualifiedName:
                    case SyntaxKind.SetAccessorDeclaration:
                    case SyntaxKind.StringLiteralToken:
                    case SyntaxKind.WhileStatement:
                    case SyntaxKind.PropertyDeclaration:
                    case SyntaxKind.CommaToken:
                    case SyntaxKind.FieldDeclaration:
                    case SyntaxKind.EqualsEqualsToken:
                    case SyntaxKind.BreakStatement:
                    case SyntaxKind.EqualsExpression:
                    case SyntaxKind.IfStatement:
                    case SyntaxKind.SwitchSection:
                    case SyntaxKind.NumericLiteralExpression:
                    case SyntaxKind.NumericLiteralToken:
                    case SyntaxKind.EqualsToken:
                    case SyntaxKind.EqualsValueClause:
                    case SyntaxKind.VariableDeclaration:
                    case SyntaxKind.VariableDeclarator:
                    case SyntaxKind.LocalDeclarationStatement:
                    case SyntaxKind.EndOfFileToken:
                    case SyntaxKind.DotToken:
                    case SyntaxKind.OpenParenToken:
                    case SyntaxKind.CloseParenToken:
                    case SyntaxKind.OpenBraceToken:
                    case SyntaxKind.CloseBraceToken:
                    case SyntaxKind.SemicolonToken:
                    case SyntaxKind.UsingDirective:
                    case SyntaxKind.SimpleMemberAccessExpression:
                    case SyntaxKind.ThisExpression:
                    case SyntaxKind.ClassDeclaration:
                    case SyntaxKind.MethodDeclaration:
                    case SyntaxKind.PredefinedType:
                    case SyntaxKind.ParameterList:
                    case SyntaxKind.Block:
                    case SyntaxKind.ExpressionStatement:
                    case SyntaxKind.Parameter:
                    case SyntaxKind.SwitchStatement:
                    case SyntaxKind.CaseSwitchLabel:
                    case SyntaxKind.BaseList:
                    case SyntaxKind.SimpleBaseType:
                    case SyntaxKind.ObjectCreationExpression:
                    case SyntaxKind.TypeOfExpression:
                        break;
                    case SyntaxKind.InvocationExpression:
                        break;
                    //default:
                        //if (kind.ToString().EndsWith("Keyword"))
                        //    break;
                        throw new NotImplementedException();
                }
                foreach (var token in node.ChildNodesAndTokens())
                    Tokenize(token, source);
            }

        }
    }
}
