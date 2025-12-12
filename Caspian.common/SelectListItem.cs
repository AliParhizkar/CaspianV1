namespace Caspian.Common
{
    public class SelectListItem
    {
        public SelectListItem()
        {

        }

        public static IList<SelectListItem> CreateList(params string[] TextArray)
        {
            var list = new List<SelectListItem>();
            for (var index = 0; index < TextArray.Length; index++)
                list.Add(new SelectListItem(index.ToString(), TextArray[index]));
            return list;
        }

        public SelectListItem(string value, string text, bool disabled = false)
        {
            Text = text;
            Value = value;
            Disabled = disabled;
        }

        public string Text { get; set; }

        public string Value { get; set; }

        public bool Disabled { get; set; }
    }
}