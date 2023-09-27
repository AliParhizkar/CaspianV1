namespace Engine.Service.CodeTokenizer
{
    public class TokenData
    {

        public TokenData() 
        {
        
        }
        
        public TokenData(int line, int startIndex, int length) 
        {
            Line = line;
            StartIndex = startIndex;
            Length = length;
            Kind = TokenKind.Simple;
        }

        public TokenData(int line, int startIndex, int length, TokenKind kind)
        {
            Line = line;
            StartIndex = startIndex;
            Length = length;
            Kind = kind;
        }

        public int Line { get; set; }

        public int StartIndex { get; set; }

        public int Length { get; set; }

        public TokenKind Kind { get; set; }
    }
}
