using Newtonsoft.Json;

namespace Caspian.UI
{
    public class SelectListItem
    {
        public SelectListItem(string value, string text, bool disabled = false)
        {
            Text = text;
            Value = value;
            Disabled = disabled;
        }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("disabled")]
        public bool Disabled { get; set; }
    }
}
