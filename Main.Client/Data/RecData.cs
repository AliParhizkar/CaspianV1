namespace Caspian.Client.Data
{
    public class RecData
    {
        public double Left { get; set; }
        
        public double Top { get; set; }
        
        public double Width { get; set; }

        public double Height { get; set; }
    }

    public class ParameterData
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
