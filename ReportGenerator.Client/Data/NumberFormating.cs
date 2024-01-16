using Caspian.Common;
using System.ComponentModel;

namespace Caspian.Report
{
    public class NumberFormating
    {
        [DisplayName("Digit Group ")]
        public bool DigitGroup { get; set; }

        [DisplayName("Number Digits")]
        public int? NumberDigits { get; set; }

        [DisplayName("کاراکتر گروهبندی")]
        public GroupNumberChar? GroupNumberChar { get; set; }

        [DisplayName("Decimal Char")]
        public DecimalChar? DecimalChar { get; set; }
    }
}
