using Caspian.Common.Client;

namespace Caspian.Report.Data
{
    public class RecData
    {
        public double Left { get; set; }
        
        public double Top { get; set; }
        
        public double Width { get; set; }

        public double Height { get; set; }
    }

    public class ParameterData: SelectListItem
    {
        public ParameterData()
        {

        }

        public ParameterData(string value, string text):
            base(value, text)
        {

        }

        public ParameterFormatType? ParameterFormatType { get; set; }
    }

    public enum ParameterFormatType: byte
    {
        Integer,
        Number,
        Date,
        String,
        Time
    }
}
