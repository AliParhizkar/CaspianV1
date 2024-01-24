using System.ComponentModel;

namespace Caspian.Report.Data
{
    public class NumberFormating
    {
        [DisplayName("Digit Group ")]
        public bool DigitGroup { get; set; }

        [DisplayName("Number Digits")]
        public int? NumberDigits { get; set; }

        [DisplayName("Grouping Char")]
        public GroupNumberChar? GroupNumberChar { get; set; }

        [DisplayName("Decimal Char")]
        public DecimalChar? DecimalChar { get; set; }
    }
}
