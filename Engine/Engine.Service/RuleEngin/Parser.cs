
using Caspian.Common;

namespace Caspian.Engine
{
    public class Parser
    {
        private IList<Token> _Tokens;
        public Parser(IList<Token> tokens)
        {
            _Tokens = tokens;
        }

        public void Parse(bool checkForFinal = false)
        {
            StateMachin stateMachin = new StateMachin();
            foreach (var token in _Tokens)
                stateMachin.Move(token.TokenKind);
            if (checkForFinal && stateMachin.CurentState.Status != StateStatus.Final)
                throw new CaspianException("فرمول ناتمام است");
            IfElse(checkForFinal);
            BraketIfThen();
            BraketThenElse();
            Braket(checkForFinal);
        }

        public IList<TokenKind> ValidTokenKinds()
        {
            StateMachin stateMachin = new StateMachin();
            foreach (var token in _Tokens)
                stateMachin.Move(token.TokenKind);
            var list = stateMachin.ValidTokenKinds();
            if (_Tokens.Count(t => t.TokenKind == TokenKind.If) == 0)
                list.Remove(TokenKind.Colon);
            var ifToken = _Tokens.LastOrDefault(t => t.TokenKind == TokenKind.If);
            Token startToken = null, endToken = null;
            if (ifToken == null)
            {
                startToken = _Tokens.FirstOrDefault();
                endToken = _Tokens.LastOrDefault();
            }
            else
            {
                var thenToken = _Tokens.LastOrDefault(t => t.TokenKind == TokenKind.QuestionMark);
                if (thenToken == null)
                {
                    startToken = ifToken;
                    endToken = _Tokens.LastOrDefault();
                }
                else
                {
                    var elseToken = _Tokens.LastOrDefault(t => t.TokenKind == TokenKind.Colon);
                    if (elseToken == null)
                    {
                        startToken = thenToken;
                        endToken = _Tokens.LastOrDefault();
                    }
                    else
                    {
                        startToken= elseToken;
                        endToken = _Tokens.LastOrDefault();
                    }
                }
            }
            if (startToken != null)
            {
                var tempList = _Tokens.Where(t => t.Id >= startToken.Id && t.Id <= endToken.Id);
                if (tempList.Count(t => t.OperatorKind == OperatorType.OpenBracket) - tempList.Count(t => t.OperatorKind == OperatorType.CloseBracket) == 0)
                    list.Remove(TokenKind.CloseBracket);
            }
            if (_Tokens.Count(t => t.OperatorKind == OperatorType.QuestionMark) - _Tokens.Count(t => t.OperatorKind == OperatorType.Colon) == 0)
                list.Remove(TokenKind.Colon);
            return list;
        }

        private void IfElse(bool checkForFinal = false)
        { 
            Stack<Token> stack = new Stack<Token>();
            foreach (var token in _Tokens)
            {
                if (token.TokenKind == TokenKind.QuestionMark)
                    stack.Push(token);
                if (token.TokenKind == TokenKind.Colon)
                {
                    if (stack.Count == 0)
                        throw new Exception("", null);
                    stack.Pop();
                }
            }
            if (stack.Count > 0 && checkForFinal)
                throw new Exception("", null);
        }

        private void BraketIfThen()
        {
            Stack<Token> stack = null;
            foreach (var token in _Tokens)
            {
                if (token.TokenKind == TokenKind.If)
                    stack = new Stack<Token>();
                else
                {
                    if (stack != null)
                    {
                        if (token.TokenKind == TokenKind.QuestionMark && stack.Count > 0)
                            throw new Exception("", null);
                        if (token.TokenKind == TokenKind.OpenBracket)
                            stack.Push(token);
                        if (token.TokenKind == TokenKind.CloseBracket)
                        {
                            if (stack.Count == 0)
                                throw new Exception("", null);
                            stack.Pop();
                        }
                    }
                }
            }
        }

        private void BraketThenElse()
        {
            Stack<Token> stack = null;
            foreach (var token in _Tokens)
            {
                if (token.TokenKind == TokenKind.QuestionMark)
                    stack = new Stack<Token>();
                else
                {
                    if (stack != null)
                    {
                        if (token.TokenKind == TokenKind.Colon && stack.Count > 0)
                            throw new Exception("", null);
                        if (token.TokenKind == TokenKind.OpenBracket)
                            stack.Push(token);
                        if (token.TokenKind == TokenKind.CloseBracket)
                        {
                            if (stack.Count == 0)
                                throw new Exception("", null);
                            stack.Pop();
                        }
                    }
                }
            }
        }

        private void Braket(bool checkForFinal = false)
        { 
            Stack<Token> stack = new Stack<Token>();
            foreach (var token in _Tokens)
            {
                if (token.TokenKind == TokenKind.OpenBracket)
                    stack.Push(token);
                if (token.TokenKind == TokenKind.CloseBracket && stack.Count == 0)
                    throw new Exception("", null);
            }
            if (stack.Count > 0 && checkForFinal)
                throw new Exception("", null);
        }
    }
}
