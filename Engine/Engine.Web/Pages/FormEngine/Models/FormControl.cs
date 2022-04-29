using System;
using System.Linq;
using ReportUiModels;
using Caspian.Engine;
using Caspian.Common;
using Newtonsoft.Json;
using System.Xml.Linq;
using System.ComponentModel;
using Caspian.Common.Extension;
using System.Collections.Generic;

namespace Page.FormEngine.Models
{
    public class FormControl
    {
        public FormControl(XElement element)
        {
            Text = element.Element("Text").Value;
            if (element.Attribute("Id") != null)
                Id = Convert.ToInt32(element.Attribute("Id").Value);
            FormControlType = (FormControlType)typeof(FormControlType).GetField(element.Element("FormControlType").Value).GetValue(null);
            switch(FormControlType)
            {
                case Caspian.Engine.FormControlType.Lable:
                    if (element.Element("SpecialField") != null)
                        SpecialField = (SpecialFieldType)typeof(SpecialFieldType).GetField(element.Element("SpecialField").Value).GetValue(null);
                    break;
                case Caspian.Engine.FormControlType.TextBox:
                    InputControlType = (InputControlType)typeof(InputControlType).GetField(element.Element("InputControlType").Value).GetValue(null);
                    break;
                case Caspian.Engine.FormControlType.CheckListBox:
                case Caspian.Engine.FormControlType.DropdownList:
                    var itemsElement = element.Element("Items").Elements();
                    Items = itemsElement.Select(t => t.Value).ToList();
                    break;
            }
            MultiSelect = element.Element("MultiSelect") != null;
            if (element.Element("Border") != null)
                Border = new Border(element.Element("Border"));
            if (element.Element("BackGroundColor") != null)
                BackGroundColor = new Color(element.Element("BackGroundColor").Value);
            Position = new Position(element.Element("Position"));
            var align = element.Element("HorizontalAlign");
            if (align != null)
                HorizontalAlign = (HorizontalAlign)typeof(HorizontalAlign).GetField(align.Value).GetValue(null);
            align = element.Element("VerticalAlign");
            if (align != null)
                VerticalAlign = (VerticalAlign)typeof(VerticalAlign).GetField(align.Value).GetValue(null);
            if (element.Element("Color") != null)
                Color = new Color(element.Element("Color").Value);
            if (element.Element("Font") != null)
                Font = new Font(element.Element("Font"));
            MultiLine = element.Element("MultiLine").Value == "true";
            IsRequired = element.Element("IsRequired").Value == "true";
        }

        [JsonProperty("id")]
        public int? Id { get; set; }

        /// <summary>
        /// عنوان
        /// </summary>
        [DisplayName("عنوان"), JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }

        /// <summary>
        /// امکان انتخاب چند گزینه با هم
        /// </summary>
        [DisplayName("چند انتخابی"), JsonProperty("multiSelect")]
        public bool MultiSelect { get; set; }

        /// <summary>
        /// فیلدهای خاص
        /// </summary>
        [DisplayName("فیلدهای خاص"), JsonProperty("specialField")]
        public SpecialFieldType? SpecialField { get; set; }

        /// <summary>
        /// نوع کنترل فرم
        /// </summary>
        [DisplayName("نوع کنترل فرم"), JsonProperty("formControlType")]
        public FormControlType? FormControlType { get; set; }

        /// <summary>
        /// نوع کنترل
        /// </summary>
        [DisplayName("نوع کنترل"), JsonProperty("inputControlType")]
        public InputControlType? InputControlType { get; set; }

        /// <summary>
        /// اجبار داده
        /// </summary>
        [DisplayName("اجبار داده"), JsonProperty("isRequired")]
        public bool IsRequired { get; set; }

        /// <summary>
        /// چند خطی
        /// </summary>
        [DisplayName("چند خطی"), JsonProperty("multiLine")]
        public bool MultiLine { get; set; }

        /// <summary>
        /// عناصر
        /// </summary>
        [DisplayName("عناصر"), JsonProperty("items")]
        public IList<string> Items { get; set; }

        [JsonProperty("border")]
        public Border Border { get; set; }

        [JsonProperty("backGroundColor")]
        public Color BackGroundColor { get; set; }

        [JsonProperty("position")]
        public Position Position { get; set; }

        [JsonProperty("horizontalAlign")]
        public HorizontalAlign? HorizontalAlign { get; set; }

        [JsonProperty("verticalAlign")]
        public VerticalAlign? VerticalAlign { get; set; }

        [JsonProperty("color")]
        public Color Color { get; set; }

        [JsonProperty("font")]
        public Font Font { get; set; }

        public XElement GetXmlElement()
        {
            var element = new XElement("Control");
            if (Id.HasValue)
                element.AddAttribute("Id", Id);
            element.AddElement("Text", Text).AddElement("FormControlType", 
                FormControlType.ToString()).AddElement("IsRequired", IsRequired)
                .AddElement("MultiLine", MultiLine);
            switch (FormControlType)
            {
                case Caspian.Engine.FormControlType.TextBox:
                    element.AddElement("InputControlType", InputControlType.ToString());
                    break;
                case Caspian.Engine.FormControlType.CheckListBox:
                case Caspian.Engine.FormControlType.DropdownList:
                    element.AddElement("Items");
                    foreach (var item in Items)
                        element.Element("Items").AddElement("Item", item);
                    break;
                case Caspian.Engine.FormControlType.Lable:
                    if (SpecialField.HasValue)
                        element.AddElement("SpecialField", SpecialField.ToString());
                    break;
                case Caspian.Engine.FormControlType.Panel:
                    break;
                default:
                    throw new CaspianException("خطا:Form control type is invalid");
            }
            if (MultiSelect)
                element.AddElement("MultiSelect", true);
            if (Border != null)
                element.AddElement("Border", Border.GetXmlElement());
            if (BackGroundColor != null)
                element.AddElement("BackGroundColor", BackGroundColor.GetXmlElementValue());
            element.AddElement("Position", Position.GetXmlElement());
            if (HorizontalAlign.HasValue)
                element.AddElement("HorizontalAlign", HorizontalAlign.ToString());
            if (VerticalAlign.HasValue)
                element.AddElement("VerticalAlign", VerticalAlign.ToString());
            if (Color != null)
                element.AddElement("Color", Color.GetXmlElementValue());
            if (Font != null)
                element.AddElement("Font", Font.GetXmlElement());
            return element;
        }

        public string GetJson()
        {
            return JsonConvert.SerializeObject(this);
            //var str = new StringBuilder();
            //str.Append("{formControlType:" + FormControlType.ConvertToInt());
            //if (Text.HasValue())
            //    str.Append(",text:\"" + Text + "\"");
            //if (Id.HasValue)
            //    str.Append(",id:" + Id);
            //if (InputControlType.HasValue)
            //    str.Append(",inputControlType:" + InputControlType.ConvertToInt());
            //if (MultiSelect)
            //    str.Append(",multiSelect:true");
            //if (Border != null)
            //    str.Append(",border:" + Border.GetJson());
            //if (BackGroundColor != null)
            //    str.Append(",backGroundColor:" + BackGroundColor.GetJson());
            //if (SpecialField.HasValue)
            //    str.Append(",specialField:" + SpecialField.ConvertToInt());
            //str.Append(",position:" + Position.GetJson());
            //if (HorizontalAlign.HasValue)
            //    str.Append(",horizontalAlign:" + HorizontalAlign.ConvertToInt());
            //if (VerticalAlign.HasValue)
            //    str.Append(",verticalAlign:" + VerticalAlign.ConvertToInt());
            //if (Value.HasValue())
            //    str.Append(",value:\"" + System.Uri.EscapeDataString(Value) + "\"");
            //if (Items != null && Items.Count > 0)
            //{
            //    str.Append(",items:[");
            //    var isFirst = true;
            //    foreach (var item in Items)
            //    {
            //        if (!isFirst)
            //            str.Append(",");
            //        str.Append("\"" + item + "\"");
            //        isFirst = false;
            //    }
            //    str.Append("]");
            //}
            //if (Color != null)
            //    str.Append(",color:" + Color.GetJson());
            //if (Font != null)
            //    str.Append(",font:" + Font.GetJson());
            //if (MultiLine)
            //    str.Append(",multiLine:true");
            //if (IsRequired)
            //    str.Append(",isRequired:true");
            //str.Append("}");
            //return str.ToString();
        }
    }
}