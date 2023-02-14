using Newtonsoft.Json;
using System.Collections.Generic;

namespace Caspian.UI
{
    public class SelectListItem
    {
        public SelectListItem()
        {

        }

        public static IList<SelectListItem> CreateList(params string[] TextArray)
        {
            var list = new List<SelectListItem>();
            for (var index = 1; index <= TextArray.Length; index++)
                list.Add(new SelectListItem(index.ToString(), TextArray[index - 1]));
            return list;
        }

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
