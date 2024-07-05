using System.Text.Json;

namespace Caspian.Report.Data
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

        public string Size { get; set; }

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
                if (Size != null)
                    str += $"font-size:{Size}px;";
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
}
