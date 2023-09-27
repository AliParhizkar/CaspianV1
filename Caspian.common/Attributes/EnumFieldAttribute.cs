using System;

namespace Caspian.Common.Attributes
{
    public class EnumTypeAttribute: Attribute
    {
        public string Title { get; set; }

        public bool IsBitwise { get; set; }
    }
}
