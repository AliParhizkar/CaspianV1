namespace Engine.Service.CaspianCodeEditor
{
    public class TabCompletionResult : IResponse
    {
        public TabCompletionResult() { }

        public virtual string Suggestion { get; set; }

        public virtual string Description { get; set; }

        public CompletionItemKind CompletionItemKind { get; set; }
    }

    public enum CompletionItemKind
    {
        Method = 0,

        Field = 3,

        Property = 9,

        Operator = 11,

        Class = 5,

        Delegate = 10,

        Structure = 6,

        Keyword = 17,

        Enum = 15,

        Snippet = 27,

        Interface = 7,

        Namespace = 8,

        TypeParameter = 24,

        Parameter = 21,

        EnumMember = 22,

        Local = 30,

        Constant = 31,

        ExtensionMethod = 1
    }
}
