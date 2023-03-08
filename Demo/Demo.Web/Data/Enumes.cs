using Caspian.Common.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Demo.Model
{
    public enum NumericType
    {
        /// <summary>
        /// عدد صحیح
        /// </summary>
        [Display(Name = "عدد صحیح")]
        Int,

        /// <summary>
        /// عدد اعشاری
        /// </summary>
        [Display(Name = "عدد اعشاری")]
        Decimal
    }
}
