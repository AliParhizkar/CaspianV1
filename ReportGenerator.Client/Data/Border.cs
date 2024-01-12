using Caspian.Common;

namespace Caspian.Report
{
    public class BorderClient
    {
        public BorderKind BorderType { get; set; }

        public BorderStyle BorderStyle { get; set; }

        public int BorderWidth { get; set; }

        public string BorderColor { get; set; }

        public string Style 
        {
            get
            {
                if (BorderType == 0)
                    return "border:none";

                return "";
            }
        }
    }
}
