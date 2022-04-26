using System;

namespace Caspian.Common
{
    public class UniqueAttribute : Attribute
    {
        internal string ErrorMessage { get; set; }
        internal string OtherField1 { get; set; }
        internal string OtherField2 { get; set; }
        internal string OtherField3 { get; set; }

        public UniqueAttribute(string errorMessage, string otherField1 = null, string otherField2 = null, string otherField3 = null)
        {
            ErrorMessage = errorMessage;
            OtherField1 = otherField1;
            OtherField2 = otherField2;
            OtherField3 = otherField3;
        }
    }
}
