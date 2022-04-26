using System;

namespace Caspian.Engine
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ReportTitleAttribute : Attribute
    {
        public string FaTite { get; private set; }

        public int Id { get; private set; }

        public ReportTitleAttribute(string faTitle, int id  = 1) 
        {
            FaTite = faTitle;  
            Id = id;
        }
    }

    /// <summary>
    /// این <see cref="Attribute"/> یک کلاس را ساخت گزارشهای پویا مشخص می نماید
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ReportClassAttribute : Attribute
    { 
        
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class ReportMethodAttribute : Attribute
    {
        public string FaTitle { get; private set; }

        public int Id { get; private set; }

        public ReportMethodAttribute(string faTitle)
        {
            FaTitle = faTitle;
        }
    }
}
