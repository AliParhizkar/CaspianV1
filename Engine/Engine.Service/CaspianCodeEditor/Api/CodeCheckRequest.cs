namespace Engine.Service.CaspianCodeEditor
{
    public class CodeCheckRequest : IRequest
    {

        public virtual string Code { get; set; }

        public virtual string[] Assemblies { get; set; }
    }
}
