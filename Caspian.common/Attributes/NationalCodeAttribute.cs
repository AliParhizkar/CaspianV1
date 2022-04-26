using System;
using System.ComponentModel.DataAnnotations;

namespace Caspian.Common.Attributes
{

    public class NationalCodeAttribute : ValidationAttribute
    {
        public char DelimiterChar { get; set; }
        public override bool IsValid(object value)
        {
            if (value == null)
                return true;
            ErrorMessage = "کد ملی نامعتبر می باشد.";
            var str = value.ToString();
            if (DelimiterChar != '\0')
            {
                if (str.Length != 12 || str[3] != DelimiterChar || str[10] != DelimiterChar)
                    return false;
                str = str.Replace(DelimiterChar.ToString(), "");
            }
            else
                if (str.Length != 10)
                return false;
            var sum = 0;
            for (var i = 0; i < 9; i++)
            {
                int chrValue = 0;
                if (Int32.TryParse(str[i].ToString(), out chrValue))
                    sum += (10 - i) * chrValue;
                else
                    return false;
            }
            var rem = sum % 11;
            if (rem >= 2)
                rem = 11 - rem;
            var flag = rem.ToString() == str[9].ToString();
            if (flag)
                ErrorMessage = null;
            return flag;
        }
    }
}
