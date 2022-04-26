using Caspian.Common.Attributes;

namespace Demo.Model
{
    public enum NumericType
    {
        /// <summary>
        /// عدد صحیح
        /// </summary>
        [EnumField("عدد صحیح")]
        Int,

        /// <summary>
        /// عدد اعشاری
        /// </summary>
        [EnumField("عدد اعشاری")]
        Decimal
    }
}
