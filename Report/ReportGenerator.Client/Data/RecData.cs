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

    public class ParamtereData: SelectListItem
    {
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
