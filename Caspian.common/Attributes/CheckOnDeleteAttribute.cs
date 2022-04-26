using System;

namespace Caspian.Common
{
    [AttributeUsage(AttributeTargets.Property)]
    public class CheckOnDeleteAttribute : Attribute
    {
        internal string ErrorMessage { get; set; }

        internal string PropertyName { get; set; }

        internal bool Check { get; set; }

        public CheckOnDeleteAttribute(bool check = false)
        {
            Check = check;
        }

        public CheckOnDeleteAttribute(string thisMessage, string propertyName = null)
        {
            ErrorMessage = thisMessage;
            PropertyName = propertyName;
            Check = true;
        }
    }
}
