using System;

namespace Caspian.Common.Attributes
{
    public class EnumTypeAttribute: Attribute
    {
        public string Title { get; set; }

        public bool Isbitw { get; set; }
    }
}
