using Caspian.Common.Extension;
using System.ComponentModel.DataAnnotations;

namespace Caspian.Common.Attributes
{
    public class NumberAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null)
                return true;
            var num = value.ToString();
            if (!num.HasValue())
                return true;
            for (var i = 0; i < num.Length; i++)
                if (num[i] < '0' || num[i] > '9')
                    return false;
            return true;
        }
    }
}
