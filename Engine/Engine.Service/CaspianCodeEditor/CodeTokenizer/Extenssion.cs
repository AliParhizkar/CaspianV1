using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Engine.Service.CodeTokenizer
{
    public static class Extenssion
    {
        public static bool IsKindOf(this SyntaxNode syntaxNode, params SyntaxKind[] syntaxKinds)
        {
            var flag = false;
            foreach (var kind in syntaxKinds)
                if (syntaxNode.IsKind(kind))
                {
                    flag = true;
                    break;
                }
            return flag;
        }

        public static TokenData GetNodeData(this SyntaxNodeOrToken node, TokenKind kind)
        {
            var linePosition = node.GetLocation().GetMappedLineSpan().StartLinePosition;
            return new TokenData 
            {
                Line = linePosition.Line,
                StartIndex = linePosition.Character,
                Kind = kind,
                Length = node.Span.Length
            };
        }

        public static TokenData GetTokenData(this SyntaxNode node, TokenKind kind)
        {
            var linePosition = node.GetLocation().GetMappedLineSpan().StartLinePosition;
            return new TokenData
            {
                Line = linePosition.Line,
                StartIndex = linePosition.Character,
                Kind = kind,
                Length = node.Span.Length
            };
        }

        public static IList<TokenData> AddCommentData(this SyntaxTrivia syntax, int[] linesLength)
        {
            var kind = syntax.Kind();
            var mainTokens = new List<TokenData>();
            switch (kind)
            {
                case SyntaxKind.SingleLineCommentTrivia:
                    var location = syntax.GetLocation().GetLineSpan().StartLinePosition;
                    mainTokens.Add(new TokenData(location.Line, location.Character, syntax.Span.End - syntax.SpanStart, TokenKind.DocumentGreen));
                    return mainTokens;
                case SyntaxKind.SingleLineDocumentationCommentTrivia:
                    /// 1111111111
                    var content = (syntax.GetStructure() as DocumentationCommentTriviaSyntax);
                    var tokens = new List<TokenData>();
                    foreach(var xml in content.Content)
                    {
                        var children = xml.ChildNodes();
                        if (children.Any())
                        {
                            foreach (var child in children)
                            {
                                var xmlKind = child.Kind();
                                switch(xmlKind)
                                {
                                    case SyntaxKind.XmlElementStartTag:
                                        if (child is XmlElementStartTagSyntax startTagSyntax)
                                        {
                                            foreach(var attr in startTagSyntax.Attributes)
                                            {
                                                var attrToken = attr.ChildNodesAndTokens().SingleOrDefault(t => t.IsKind(SyntaxKind.IdentifierName));
                                                if (attrToken != null)
                                                    tokens.Add(attrToken.GetNodeData(TokenKind.DocumentBlue));
                                            }
                                        }
                                        break;
                                    case SyntaxKind.XmlText:
                                        foreach(var child1 in child.ChildNodesAndTokens())
                                        {
                                            if (child1.IsKind(SyntaxKind.XmlTextLiteralToken) && child1.SpanStart + 1 < child1.FullSpan.End)
                                            {
                                                tokens.Add(child1.GetNodeData(TokenKind.DocumentGreen));
                                            }
                                        }
                                        break;
                                }
                            }
                        }
                        else
                        {
                            location = xml.GetLocation().GetLineSpan().StartLinePosition;
                            mainTokens.Add(new TokenData(location.Line, location.Character - 3, 3, TokenKind.DocumentGray));
                            mainTokens.Add(new TokenData(location.Line, location.Character, xml.Span.End - xml.SpanStart, TokenKind.DocumentGreen));
                            return mainTokens;
                        }
                    }
                    var mapping = content.GetLocation().GetMappedLineSpan();
                    var startLine = mapping.StartLinePosition.Line;
                    var endLine = mapping.EndLinePosition.Line;
                    
                    for (var line = startLine; line < endLine; line++)
                    {
                        var tempTokens = tokens.Where(t => t.Line == line).ToList();
                        if (tempTokens.Any())
                        {
                            var index = 0;
                            foreach (var token in tempTokens)
                            {
                                if (token.StartIndex > index)
                                {
                                    mainTokens.Add(new TokenData(line, index, token.StartIndex - index, TokenKind.DocumentGray));
                                    index = token.StartIndex;
                                }
                                mainTokens.Add(token);
                                index += token.Length;
                            }
                            if (index + 1 < linesLength[line])
                                mainTokens.Add(new TokenData(line, index, linesLength[line] - index, TokenKind.DocumentGray));
                        }
                        else
                            mainTokens.Add(new TokenData(line, 0, linesLength[line], TokenKind.DocumentGray));
                    }
                    
                    break;
                case SyntaxKind.MultiLineCommentTrivia:
                    var positionSpan = syntax.GetLocation().GetMappedLineSpan();
                    if (positionSpan.StartLinePosition.Line == positionSpan.EndLinePosition.Line)
                    {
                        var lenth = positionSpan.EndLinePosition.Character - positionSpan.StartLinePosition.Character;
                        mainTokens.Add(new TokenData(positionSpan.StartLinePosition.Line, positionSpan.StartLinePosition.Character, lenth, TokenKind.DocumentGreen));
                        return mainTokens;
                    }
                    var len = linesLength[positionSpan.StartLinePosition.Line] - positionSpan.StartLinePosition.Character;
                    mainTokens.Add(new TokenData(positionSpan.StartLinePosition.Line, positionSpan.StartLinePosition.Character, len, TokenKind.DocumentGreen));
                    for (var i = positionSpan.StartLinePosition.Line + 1; i < positionSpan.EndLinePosition.Line; i++)
                        mainTokens.Add(new TokenData(i, 0, linesLength[i], TokenKind.DocumentGreen));
                    mainTokens.Add(new TokenData(positionSpan.EndLinePosition.Line, 0, positionSpan.EndLinePosition.Character, TokenKind.DocumentGreen));
                    return mainTokens;
                default:
                    break;
            }
            return mainTokens;
        }

        public static bool IsDocument(this SyntaxTrivia syntax)
        {
            var syntaxKind = syntax.Kind();
            return syntaxKind != SyntaxKind.EndOfLineTrivia && syntaxKind != SyntaxKind.WhitespaceTrivia;
        }
    }
}

