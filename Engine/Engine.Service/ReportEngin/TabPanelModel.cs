using Caspian.Common;
using System.Xml.Linq;
using System.ComponentModel;
using Caspian.Common.Extension;
using System.ComponentModel.DataAnnotations;

namespace Caspian.Engine
{
    /// <summary>
    /// مشخصات TabPanel مربوط به هر گزارش
    /// </summary>
    public class TabPanelModel
    {
        public TabPanelModel()
        {
            Controls = new List<ReportControlModel>();
        }

        public TabPanelModel(Type type, XElement element)
        {
            Id = Convert.ToInt32(element.Attribute("Id").Value);
            Title = element.Attribute("Title").Value;
            var controls = element.Element("Controls").Elements();
            Controls = controls.Select(t => new ReportControlModel(type, t)).ToList();
        }

        public int Id { get; set; }

        /// <summary>
        /// عنوان TabPanel
        /// </summary>
        [DisplayName("عنوان"), Required]
        public string Title { get; set; }

        /// <summary>
        /// مشخصات کنترلهای TabPanel
        /// </summary>
        public IList<ReportControlModel> Controls { get; set; }

        public XElement GetXml()
        {
            var element = new XElement("Panel").AddAttribute("Id", Id)
                .AddAttribute("Title", Title).AddElement("Controls");
            var controls = element.Element("Controls");
            controls.AddAttribute("Count", Controls.Count);
            foreach (var control in Controls)
                controls.AddElement(control.GetXml());
            return element;
        }

        public string ToJson(Type type)
        {
            var str = "{id:" + Id;
            if (Title.HasValue())
                str += ",title:\"" + Title + "\"";
            str += ",controls:[";
            var isFirstTime = true;
            foreach (var control in Controls)
            {
                if (isFirstTime)
                    isFirstTime = false;
                else
                    str += ",";
                str += control.ToJson(type);
            }
            str += "]}";
            return str;
        }
    }
}
