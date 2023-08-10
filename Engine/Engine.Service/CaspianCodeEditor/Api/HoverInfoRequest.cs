namespace Engine.Service.CaspianCodeEditor
{
    public class HoverInfoRequest : IRequest
    {
        public HoverInfoRequest()
        { }

        public HoverInfoRequest(string code, int position, string[] assemblies)
        {
            Code = code;
            Position = position;
            Assemblies = assemblies;
        }

        public virtual string Code { get; set; }

        public virtual int Position { get; set; }

        public virtual string[] Assemblies { get; set; }

    }
}
