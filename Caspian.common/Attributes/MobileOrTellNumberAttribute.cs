using System.ComponentModel.DataAnnotations;

namespace Caspian.Common.Attributes
{
    public class MobileOrTellNumberAttribute : ValidationAttribute
    {
        private TellNumberType _NumberType;
        public MobileOrTellNumberAttribute(TellNumberType numberType)
        {
            _NumberType = numberType;
        }

        public override bool IsValid(object value)
        {
            if (value != null)
            {
                var num = value.ToString();
                if (_NumberType == TellNumberType.ShortTell && num.Length != 8)
                    return false;
                if (num.Length != 11)
                    return false;
                for (var i = 0; i < num.Length; i++)
                    if (num[i] < '0' || num[i] > '9')
                        return false;
            }
            return true;
        }
    }

    public enum TellNumberType
    {
        Mobile,
        Tell,
        ShortTell,
        MobileOrTell,
        MobileOrShortTell
    }
}
