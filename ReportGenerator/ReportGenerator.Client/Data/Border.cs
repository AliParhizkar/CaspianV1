using Caspian.Common;

namespace Caspian.Report
{
    public class Border
    {
        public Border()
        {
            Width = 1;
            BorderStyle = BorderStyle.Solid;
            Color = new Color();
        }

        public int Width { get; set; }

        public BorderStyle BorderStyle { get; set; }

        public Color Color { get; set; }

        public BorderKind BorderKind { get; set; }

        public string Style
        {
            get
            {
                if (BorderKind == 0)
                    return "";
                string border = $"{Width}px {BorderStyle.ToString().ToLower()} {Color.ColorString};";
                if (BorderKind == BorderKind.Top)
                    return $"border-top:{border}";
                if (BorderKind == BorderKind.Bottom)
                    return $"border-bottom:{border}";
                if (BorderKind == BorderKind.Left)
                    return $"border-left:{border}";
                if (BorderKind == BorderKind.Right)
                    return $"border-right:{border}";
                var str = $"border:{border}";
                if ((BorderKind & BorderKind.Left) == 0)
                    str += "border-left:none;";
                if ((BorderKind & BorderKind.Right) == 0)
                    str += "border-right:none;";
                if ((BorderKind & BorderKind.Top) == 0)
                    str += "border-top:none;";
                if ((BorderKind & BorderKind.Bottom) == 0)
                    str += "border-bottom:none;";
                return str;
            }
        }
    }
}
