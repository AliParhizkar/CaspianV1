using System;

namespace Caspian.Common
{
    [AttributeUsage(AttributeTargets.Property)]
    public class CheckOnInsertAttribute : Attribute
    {
        public string Message { get; set; }

        public CheckOnInsertAttribute(string message = null)
        {
            this.Message = message;
        }
    }
}
