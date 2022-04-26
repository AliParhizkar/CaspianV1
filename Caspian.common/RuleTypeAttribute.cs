using System;

namespace Caspian.Engine
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RuleTypeAttribute: Attribute
    {
                /// <summary>
        /// عنوان فارسی پارامتر
        /// </summary>
        public string Title { get; set; }

        public RuleTypeAttribute(string title)
        {
            Title = title;
        }
    }
}
