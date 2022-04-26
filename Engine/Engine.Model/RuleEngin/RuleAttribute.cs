using System;

namespace Caspian.Engine
{
    /// <summary>
    /// برای تعیین یک Property بصورت پارامتریک
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class RuleAttribute : Attribute
    {
        /// <summary>
        /// عنوان فارسی پارامتر
        /// </summary>
        public string Title { get; set; }

        public RuleAttribute()
        {

        }

        public RuleAttribute(string title)
        {
            Title = title;
        }
    }
}
