using Caspian.Common;

namespace Caspian.Report
{
    public class AlignmentClient
    {
        public AlignmentClient() 
        {
            VerticalAlign = VerticalAlign.Middle;
            HorizontalAlign = HorizontalAlign.Justify;
        }

        public VerticalAlign VerticalAlign { get; set; }

        public HorizontalAlign HorizontalAlign { get; set;}

        public string Style
        {
            get
            {
                var str = "";

                return str;
            }
        }
    }
}
