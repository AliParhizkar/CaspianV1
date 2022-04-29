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

        public string Url { get; set; }

        public string DisplayField { get; set; }

        public string ValueField { get; set; }

        public CompositionMethodType? Type { get; set; }

        public int? RuleId { get; set; }

        public int? DynamicParameterId { get; set; }

        public FilteringControlType? FilteringControlType { get; set; }

        /// <summary>
        /// Regular expression pattern
        /// </summary>
        public string Pattern { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string MaskText { get; set; }
    }
}
