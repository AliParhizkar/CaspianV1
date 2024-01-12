using System.Text.Json;
using System.Xml.Linq;
using System.Globalization;

namespace Caspian.Common
{
    public class Font
    {
        public bool Bold { get; set; }

        public bool Italic { get; set; }

        public bool UnderLine { get; set; }

        public string Family { get; set; }

        public double Size { get; set; }


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
        public double Width { get; set; }

        public BorderStyle Style { get; set; }

        public Color Color { get; set; }

        public BorderKind BorderKind { get; set; }

        public string GetJson()
        {
            return JsonSerializer.Serialize(this, new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            });
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
                color = color.Substring(1, color.Length - 2);
                var array = color.Split(':');
                ColorString = String.Format("{0:X}", Convert.ToInt32(array[0]));
                if (ColorString.Length == 1)
                    ColorString = '0' + ColorString;

                var str = String.Format("{0:X}", Convert.ToInt32(array[1]));
                if (str.Length == 1)
                    str = '0' + str;
                ColorString += str;
                str = String.Format("{0:X}", Convert.ToInt32(array[2]));
                if (str.Length == 1)
                    str = '0' + str;
                ColorString += str;
                ColorString = '#' + ColorString;
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