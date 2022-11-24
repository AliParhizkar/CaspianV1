using System;

namespace Caspian.Engine
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ReportFieldAttribute : Attribute
    {
        public ReportFieldAttribute()
        {
            Where = WhereFieldType.Buffer;
            OrderBy = true;
        }

        public ReportFieldAttribute(string title)
        {
            Title = title;
            OrderBy = true;
            Where = WhereFieldType.Buffer;
        }

        public string Title { get; set; }

        public bool OrderBy { get; set; }

        public WhereFieldType Where { get; set; }

        public int StartIndex { get; set; }

        public int Length { get; set; }

        public string DisplayField { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string MaskedText { get; set; }
    }

    public enum WhereFieldType
    {
        True = 1,
        False,
        Buffer
    }
}
