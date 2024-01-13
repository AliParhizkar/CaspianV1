using System.Text.Json;
using System.Globalization;

namespace Caspian.Common
{
    public class Font
    {
        public Font() 
        {
            Color = new Color();
        }

        public bool Bold { get; set; }

        public bool Italic { get; set; }

        public bool UnderLine { get; set; }

        public string Family { get; set; }

        public double Size { get; set; }

        public Color Color { get; set; }

        public string Style
        {
            get
            {
                var str = "";
                if (Bold)
                    str += "font-weight: bold;";
                if (Italic)
                    str += "font-style: italic;";
                if (UnderLine)
                    str += "text-decoration: underline;";
                str += $"color:{Color.ColorString};";
                return str;
            }
        }

        public string GetJson()
        {
            return JsonSerializer.Serialize(this, new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            });
        }
    }

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

    public class Color
    {
        public Color(string color)
        {
            if (color.ToLower() == "transparent" || color.ToLower().StartsWith("rgba"))
                ColorString = "Transparent";
            else
            {
                ColorString = color;
            }
        }

        public string ColorString { get; set; }

        public Color()
        {
            ColorString = "#000000";
        }

        private int Red
        {
            get
            {
                if (ColorString.StartsWith("#"))
                    return int.Parse(ColorString.Substring(1, 2), NumberStyles.HexNumber);
                var str = ColorString.Trim().ToLower();
                str = str.Substring(4, str.Length - 5);
                return int.Parse(str.Split(',')[0]);
            }
        }

        private int Green
        {
            get
            {
                if (ColorString.StartsWith("#"))
                    return int.Parse(ColorString.Substring(3, 2), NumberStyles.HexNumber);
                var str = ColorString.Trim();
                str = str.Substring(4, str.Length - 5);
                return int.Parse(str.Split(',')[1]);
            }
        }

        private int Blue
        {
            get
            {
                if (ColorString.StartsWith("#"))
                    return int.Parse(ColorString.Substring(5, 2), NumberStyles.HexNumber);
                var str = ColorString.Trim();
                str = str.Substring(4, str.Length - 5);
                return int.Parse(str.Split(',')[2]);
            }
        }

        public string GetJson()
        {
            if (ColorString == null || ColorString.ToLower() == "transparent")
                ColorString = "Transparent";
            return JsonSerializer.Serialize(this, new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            });
        }
    }

    public class Position
    {

        public Position()
        {

        }

        public Position(decimal left, decimal top, decimal width, decimal height)
        {
            Left = left;
            Top = top;
            Width = width;
            Height = height;
        }

        public decimal Left { get; set; }

        public decimal Top { get; set; }

        public decimal Width { get; set; }

        public decimal Height { get; set; }

        public string GetJson()
        {
            return JsonSerializer.Serialize(this, new JsonSerializerOptions() 
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            });
        }
    }
}