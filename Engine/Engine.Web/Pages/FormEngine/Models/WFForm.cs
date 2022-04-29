using System;
using System.Linq;
using ReportUiModels;
using System.Xml.Linq;
using Newtonsoft.Json;
using Caspian.Common.Extension;
using System.Collections.Generic;

namespace Page.FormEngine.Models
{
    public class FormBond 
    {
        public FormBond(XElement element)
        {
            Text = element.Element("Text").Value;
            Width = Convert.ToDecimal(element.Element("Width").Value);
            Height = Convert.ToDecimal(element.Element("Height").Value);
            Controls = element.Element("Controls").Elements()
                .Select(t => new FormControl(t)).ToList();
        }

        public FormBond() 
        {
            Controls = new List<FormControl>();
        }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("width")]
        public decimal Width { get; set; }

        [JsonProperty("height")]
        public decimal Height { get; set; }

        [JsonProperty("BackGroundColor")]
        public Color BackGroundColor { get; set; }

        [JsonProperty("controls")]
        public IList<FormControl> Controls { get; set; }

        public XElement GetXmlElement()
        {
            var element = new XElement("Bond");
            element.AddElement("Text", Text).AddElement("Width", Width)
                .AddElement("Height", Height).AddElement("Controls")
                .AddElement("BackGroundColor", BackGroundColor.GetXmlElementValue());
            foreach (var control in Controls)
                element.Element("Controls").AddElement(control.GetXmlElement());
            return element;
        }

        public string GetJson()
        {
            return JsonConvert.SerializeObject(this);
            //var str = new StringBuilder();
            //str.Append("{text:\"" + Text + "\"");
            //str.Append(",width:" + Width);
            //str.Append(",height:" + Height);
            //str.Append(",controls:[");
            //var isFirst = true;
            //foreach (var control in Controls)
            //{
            //    if (!isFirst)
            //        str.Append(",");
            //    str.Append(control.GetJson());
            //    isFirst = false;
            //}
            //str.Append("]}");
            //return str.ToString();
        }
    }


}