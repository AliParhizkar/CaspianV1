namespace Caspian.Engine
{
    /// <summary>
    /// مشخصات نودهای گزارش در درخت <see cref="Select"/> و یا در درخت <see cref="Where"/>
    /// </summary>
    public class ReportNode
    {
        public string TitleEn { get; set; }

        public string TitleFa { get; set; }

        public bool Selected { get; set; }

        public bool Grouping { get; set; }

        public bool UseOrderBy { get; set; }

        public CompositionMethodType? Type { get; set; }

        public DynamicParameterType? DynamicParameterType { get; set; }

        public int? RuleId { get; set; }

        public int? DynamicParameterId { get; set; }
    }
}
