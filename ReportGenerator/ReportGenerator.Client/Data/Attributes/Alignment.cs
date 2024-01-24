namespace Caspian.Report.Data
{
    public class Alignment
    {
        public Alignment() 
        {
            VerticalAlign = VerticalAlign.Middle;
            HorizontalAlign = HorizontalAlign.Center;
        }

        public VerticalAlign VerticalAlign { get; set; }

        public HorizontalAlign HorizontalAlign { get; set;}

        public string Style
        {
            get
            {
                var str = "align-items:";
                switch(VerticalAlign)
                {
                    case VerticalAlign.Top:
                        str += "flex-start;"; break;
                    case VerticalAlign.Middle:
                        str += "center;"; break;
                    case VerticalAlign.Bottom:
                        str += "flex-end;"; break;
                }
                str += ";justify-content:";
                switch (HorizontalAlign)
                {
                    case HorizontalAlign.Left:
                    case HorizontalAlign.Justify:
                        str += "left;"; break;
                    case HorizontalAlign.Center:
                        str += "center;"; break;
                    case HorizontalAlign.Right:
                        str += "right;"; break;
                }
                return str;
            }
        }

        public string TextStyle
        {
            get
            {
                return $"text-align:{HorizontalAlign};";
            }
        }
    }
}
