using Newtonsoft.Json;
using System.Xml.Linq;
using Caspian.Common.Extension;

namespace Page.FormEngine.Models
{
    public class FormModel
    {
        public FormModel()
        {

        }

        public FormModel(XElement element)
        {
            Bond = new FormBond(element.Element("Bond"));
        }

        [JsonIgnore]
        public int Id { get; set; }

        [JsonProperty("bond")]
        public FormBond Bond { get; set; }

        public XElement GetXmlElement()
        {
            var element = new XElement("Form");
            element.AddElement(Bond.GetXmlElement());
            return element;
        }

        public string GetJson()
        {
            return JsonConvert.SerializeObject(this);
            //var str = new StringBuilder("{bond:");
            //str.Append(Bond.GetJson() + "}");
            //return str.ToString();
        }
    }
}