namespace Engine.Service.CaspianCodeEditor
{
    public class HoverInfoResult : IResponse
    {
        public HoverInfoResult() { }

        public virtual string Information { get; set; }

        public virtual int OffsetFrom { get; set; }

        public virtual int OffsetTo { get; set; }
    }
}
