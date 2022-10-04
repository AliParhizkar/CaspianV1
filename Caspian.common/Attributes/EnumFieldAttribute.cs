using System;

namespace Caspian.Common.Attributes
{
    public class EnumFieldAttribute : Attribute
    {
        public string DisplayName { get; private set; }

        public string ClassName { get; private set; }

        public EnumFieldAttribute(string displayName, string className = null)
        {
            DisplayName = displayName;
            ClassName = className;
        }
    }

    public class EnumTypeAttribute: Attribute
    {
        public string Title { get; set; }
        public EnumTypeAttribute(string title)
        {
            Title = title;
        }
    }
}
