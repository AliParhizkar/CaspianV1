using System.Xml.Linq;
using System.Globalization;
using System.Text.Json;

namespace ReportUiModels
{
    public class Font
    {
        public Font(XElement element)
        {
            var value = element.Value;
            var array = value.Split(',');
            Family = array[0];
            Size = Convert.ToDouble(array[1]);
            if (array.Length > 2)
            {
                var str = array[2];
                if (str.Contains("Bold"))
                    Bold = true;
                if (str.Contains("Italic"))
                    Italic = true;
                if (str.Contains("Underline"))
                    UnderLine = true;
            }
        }

        public Font()
        {

        }

        public bool Bold { get; set; }

        public bool Italic { get; set; }

        public bool UnderLine { get; set; }

        public string Family { get; set; }

        public double Size { get; set; }

        public XElement GetXmlElement()
        {
            Family = "Arial"; //******************************************
            var temp = "";
            if (Bold)
                temp = "Bold";
            if (Italic)
                if (temp == "")
                    temp += "Italic";
                else
                    temp += "| Italic";
            if (UnderLine)
                if (temp == "")
                    temp += "Underline";
                else
                    temp += "| Underline";
            var str = Family + ',' + Size;
            if (temp != "")
                str += ',' + temp;
            var x = new XElement("Font");
            x.Add(str);
            return x;
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
            
        }

        public Border(XElement element)
        {
            var array = element.Value.Split(';');
            var borderKindArray = array[0].Split(',');
            var borderKindValue = 0;
            for (var i = 0; i < 4; i++ )
                if (borderKindArray.Length > i)
                    borderKindValue |= (int)typeof(BorderKind).GetField(borderKindArray[i]).GetValue(null);
            this.BorderKind = (BorderKind)borderKindValue;
            Color = new Color(array[1]);
            Width = Convert.ToDouble(array[2]);
            var filed = typeof(BorderStyle).GetField(array[3]);
            if (filed != null)
                Style = (BorderStyle)filed.GetValue(null);
        }

        public double Width { get; set; }

        public BorderStyle Style { get; set; }

        public Color Color { get; set; }

        public BorderKind BorderKind { get; set; }

        public object GetXmlElement()
        {
            var str = "";
            if (BorderKind == BorderKind.All)
                str = "All";
            else
                if (BorderKind == BorderKind.None)
                    str = "None";
                else
                {
                    if (((int)BorderKind & 1) == 1)
                        str = "Top";
                    if (((int)BorderKind & 2) == 2)
                    {
                        if (str != "")
                            str += ',';
                        str += "Bottom";
                    }
                    if (((int)BorderKind & 4) == 4)
                    {
                        if (str != "")
                            str += ',';
                        str += "Left";
                    }
                    if (((int)BorderKind & 8) == 8)
                    {
                        if (str != "")
                            str += ',';
                        str += "Right";
                    }
                }
            var element = new XElement("Border");
            if (Color == null)
                Color = new Color();
            element.Add(str + ';' + Color.GetXmlElementValue() + ';' + Width + ';' + Style.ToString() + ";False;4;[0:0:0]");
            return element;
        }

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

        public string GetXmlElementValue()
        {
            if (ColorString == null || ColorString.ToLower() == "transparent" || ColorString.ToLower().StartsWith("rgba"))
                return "Transparent";
            return "[" + Red + ':' + Green + ':' + Blue + ']';
        }

        public string GetJson()
        {
            var str = ColorString;
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
        public Position(XElement element)
        {
            var str = element.Value;
            var array = str.Split(',');
            var decimalDigits = Convert.ToChar(Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator);
            var oldDecimalDigits = decimalDigits == '.' ? '/' : '.';
            Left = Convert.ToDecimal(array[0].Replace(oldDecimalDigits, decimalDigits));
            Top = Convert.ToDecimal(array[1].Replace(oldDecimalDigits, decimalDigits));
            Width = Convert.ToDecimal(array[2].Replace(oldDecimalDigits, decimalDigits));
            Height = Convert.ToDecimal(array[3].Replace(oldDecimalDigits, decimalDigits));
        }

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

        public XElement GetXmlElement()
        {
            var element = new XElement("ClientRectangle");
            element.Add(Left + "," + Top + "," + Width + "," + Height);
            return element;
        }

        public string GetJson()
        {
            return JsonSerializer.Serialize(this, new JsonSerializerOptions() 
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            });
        }
    }
}