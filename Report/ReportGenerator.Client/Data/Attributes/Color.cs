using System.Text.Json;
using System.Globalization;
using System.Text.Json.Serialization;

namespace Caspian.Report.Data
{
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

        [JsonIgnore]
        public int Red
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

        [JsonIgnore]
        public int Green
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

        [JsonIgnore]
        public int Blue
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
}
