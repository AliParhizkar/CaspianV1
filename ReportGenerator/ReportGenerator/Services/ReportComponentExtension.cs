using Caspian.Common;
using Caspian.Report;
using System.Xml.Linq;
using Caspian.Report.Data;
using Caspian.Common.Extension;

namespace ReportGenerator.Services
{
    public static class ReportComponentExtension
    {
        /// <summary>
        /// Pixels Per Centimetre
        /// </summary>
        public static double PPC { get; set; }

        public static async Task<XElement> GetXMLDocument(this PageData pageData, IServiceProvider provider)
        {
            var element = new XElement("StiSerializer").AddAttribute("version", 1.02).AddAttribute("type", "Net")
                .AddAttribute("application", "StiReport");
            element.AddElement("CalculationMode").AddContent("Interpretation");
            var dicElement = await new Dictionary(provider).GetXmlElement(pageData.ReportId);
            element.AddElement(dicElement);
            element.AddElement("EngineVersion").AddContent("EngineV2");
            element.AddElement("GlobalizationStrings").AddAttribute("isList", true).AddAttribute("count", 0);
            element.AddElement("Key", "49524a268db240eab98e84c14d67e873");
            element.AddElement("MetaTags").AddAttribute("isList", true).AddAttribute("count", 0);
            var pages = new XElement("Pages").AddAttribute("isList", true).AddAttribute("count", 1);
            pages.Add(pageData.GetXMLElement());
            element.Add(pages);
            element.AddElement("PrinterSettings").AddAttribute("Ref", 9999).AddAttribute("type", "Stimulsoft.Report.Print.StiPrinterSettings").AddAttribute("isKey", true);
            element.AddElement("ReportAlias").AddContent("Report");
            element.AddElement("ReportChanged").AddContent("5/3/2015 4:05:02 PM");
            element.AddElement("ReportCreated").AddContent("5/1/2015 7:31:06 PM");
            element.AddElement("ReportFile").AddContent("D:\\e.mrt");
            element.AddElement("ReportGuid").AddContent("7a59e6f3323042d4848703beddfe3e7b");
            element.AddElement("ReportName").AddContent("ReportName");
            element.AddElement("ReportUnit").AddContent("Centimeters");
            element.AddElement("ReportVersion").AddContent("2012.1.1300");
            element.AddElement("ScriptLanguage").AddContent("CSharp");
            element.AddElement("Styles").AddAttribute("isList", true).AddAttribute("count", 0);
            return element;
        }

        public static XElement GetXMLElement(this Border border)
        {
            var str = "";
            var kind = (int)border.BorderKind;
            if (kind == 15)
                str = "All";
            else if (kind == 0)
                str = "None";
            else
            {
                if ((kind & 1) == 1)
                    str += "Top,";
                if ((kind & 2) == 2)
                    str += "Bottom,";
                if ((kind & 4) == 4)
                    str += "Left,";
                if ((kind & 8) == 8)
                    str += "Right,";
                str = str.Substring(0, str.Length - 1); /// remove , in end
            }
            var element = new XElement("Border");
            var style = border.BorderStyle.ToString().Replace("ted", "").Replace("ed", "");
            element.Add($"{str};{border.Color.GetXMLElement()};{border.Width};{style};False;4;[0:0:0]");
            return element;
        }

        public static XElement GetXMLElement(this PageData pageData)
        {
            var page = new XElement("Page1").AddAttribute("Ref", 15).AddAttribute("type", "Page").AddAttribute("isKey", true);
            page.AddElement(pageData.Border.GetXMLElement());
            var backGroundColor = new XElement("Brush");
            if (pageData.BackGroundColor == null)
                backGroundColor.Add("Transparent");
            else
                backGroundColor.Add(pageData.BackGroundColor.GetXMLElement());
            page.Add(backGroundColor);
            page.AddElement(pageData.Bound.GetXMLElement(pageData));
            page.AddElement(new XElement("Conditions").AddAttribute("isList", true).AddAttribute("count", 0));
            page.AddElement(new XElement("Expressions").AddAttribute("isList", true).AddAttribute("count", 0));
            page.AddElement("Guid", "f1f6fa338a744ced830a2b55c8e7a5f8");
            page.AddElement("Margins", "0,0,0,0");
            page.AddElement("Name", "Page1");
            page.AddElement("PageWidth", pageData.Setting.PageWidth);
            page.AddElement("PageHeight", pageData.Setting.PageHeight);
            page.AddElement(new XElement("Report").AddAttribute("isRef", 0));
            return page;
        }

        public static XElement GetXMLElement(this BoundData boundData, PageData pageData)
        {
            var bound = new XElement("Components").AddAttribute("isList", true).AddAttribute("count", boundData.Items.Count);
            var top = 53;
            foreach ( var item in boundData.Items)
            {
                foreach(var control in item.Controls)
                {
                    control.Left -= 182;
                    control.Top -= top;
                }
                top += item.Height + 3;
                bound.AddElement(item.GetXMLElement(pageData));
            }
            return bound;
        }

        public static XElement GetXMLElement(this BoundItemData boundItemData, PageData pageData)
        {
            if (boundItemData.Table != null && boundItemData.BondType != BondType.DataHeader)
                return boundItemData.Table.GetXMLElement(pageData);
            XElement element = null;
            var boundName = "";
            switch(boundItemData.BondType)
            {
                case BondType.ReportTitle:
                    boundName = "ReportTitleBand1";
                    element = new XElement(boundName);
                    element.AddAttribute("Ref", 1).AddAttribute("type", "ReportTitleBand").AddAttribute("isKey", true);
                    break;
                case BondType.PageHeader:
                    boundName = "PageHeaderBand1";
                    element = new XElement(boundName).AddAttribute("Ref", 2).AddAttribute("type", "PageHeaderBand")
                        .AddAttribute("isKey", true);
                    break;
                case BondType.DataHeader:
                    if (boundItemData.ColumnsCount > 1)
                    {
                        boundName = "ColumnHeaderBand1";
                        element = new XElement(boundName).AddAttribute("Ref", 3).AddAttribute("type", "Stimulsoft.Report.Components.StiColumnHeaderBand").AddAttribute("isKey", true);
                    }
                    else
                    {
                        boundName = $"HeaderBand{boundItemData.BondType.ConvertToInt()}";
                        element = new XElement(boundName).AddAttribute("Ref", 3).AddAttribute("type", "HeaderBand").AddAttribute("isKey", true);
                    }
                    break;
                case BondType.FirstDataLevel:
                case BondType.SecondDataLevel:
                case BondType.ThirdDataLevel:
                    var id = boundItemData.BondType.ConvertToInt();
                    boundName = $"DataBand{id}";
                    element = new XElement(boundName).AddAttribute("Ref", id + 1).AddAttribute("type", "DataBand")
                        .AddAttribute("isKey", true);
                    break;
                case BondType.DataFooter:

                case BondType.PageFooter:
                    boundName = "PageFooterBand7";
                    element = new XElement(boundName).AddAttribute("Ref", 8).AddAttribute("type", "PageFooterBand")
                        .AddAttribute("isKey", true);
                    break;
            }
            element.Add(boundItemData.Border.GetXMLElement());
            element.AddElement("Brush", "Transparent");
            var height = boundItemData.Height;
            if (boundItemData.Table != null)
                height = boundItemData.Table.Rows.Sum(t => t.Height);
            element.AddElement(Extension.ClientRectangle(0, 0, pageData.Width, height));
            var dataLevel = boundItemData.GetDataLevel();
            if (dataLevel.HasValue)
            {
                var guId = BusinessObject.GetGuId(dataLevel.Value);
                element.AddElement("BusinessObjectGuid").AddContent(guId);
                if (dataLevel.Value > 1)
                    element.AddElement("MasterComponent").AddAttribute("isRef", dataLevel.Value + 2);
            }
            var controlsCount = boundItemData.Table != null ? 1 : boundItemData.Controls.Count;
            var controls = element.AddElement("Components").AddAttribute("isList", true).AddAttribute("count", controlsCount).Element("Components");
            if (boundItemData.Table != null)
                controls.AddElement(boundItemData.Table.GetXMLElement(pageData));
            else
                foreach (var control in boundItemData.Controls)
                    controls.AddElement(control.GetXMLElement(boundItemData));
            element.AddElement(new XElement("Conditions").AddAttribute("isList", true).AddAttribute("count", 0));
            element.AddElement(new XElement("Expressions").AddAttribute("isList", true).AddAttribute("count", 0));
            element.AddElement("Name", boundName);
            element.AddElement(new XElement("Page").AddAttribute("isRef", 15));
            element.AddElement(new XElement("Parent").AddAttribute("isRef", 15));
            return element;
        }

        public static void UpdateWidthAndHeight(this ControlData control)
        {
            var kind = (int)control.Border.BorderKind;
            if ((kind & 1) == 1)
                control.Height -= control.Border.Width;
            if ((kind & 2) == 2)
                control.Height -= control.Border.Width;
            if ((kind & 4) == 4)
                control.Width -= control.Border.Width;
            if ((kind & 8) == 8)
                control.Width -= control.Border.Width;
        }

        public static XElement GetXMLElement(this Font font)
        {
            font.Family = "Arial"; //******************************************
            font.Size = "14"; //******************************************
            var temp = "";
            if (font.Bold)
                temp = "Bold";
            if (font.Italic)
                if (temp == "")
                    temp += "Italic";
                else
                    temp += "| Italic";
            if (font.UnderLine)
                if (temp == "")
                    temp += "Underline";
                else
                    temp += "| Underline";
            var str = font.Family + ',' + font.Size;
            if (temp != "")
                str += ',' + temp;
            return new XElement("Font", str);
        }

        public static XElement GetXMLElement(this ControlData control, BoundItemData boundItem)
        {
            control.UpdateWidthAndHeight();
            var id = Extension.CreateId();
            var name = control.ControlType == ControlType.TextBox ? "Text" : "Image";
            var element = new XElement(name + (id - 20)).AddAttribute("Ref", id).AddAttribute("type", name).AddAttribute("isKey", true);
            element.Add(new XElement("Brush", control.BackgroundColor.GetXMLElement()));
            element.AddElement(Extension.ClientRectangle(control.Left, control.Top, control.Width, control.Height));
            element.AddElement(new XElement("Conditions").AddAttribute("isList", true).AddAttribute("count", 0));
            element.AddElement(new XElement("Expressions").AddAttribute("isList", true).AddAttribute("count", 0));
            element.AddElement("HorAlignment", control.Alignment.HorizontalAlign == HorizontalAlign.Justify ? "Width" : control.Alignment.HorizontalAlign.ToString());
            element.AddElement("VertAlignment", control.Alignment.VerticalAlign == VerticalAlign.Middle ? "Center" : control.Alignment.VerticalAlign.ToString());
            if (control.ControlType == ControlType.TextBox)
            {
                element.Add(control.Font.GetXMLElement());
                element.AddElement("TextBrush", control.Font.Color.GetXMLElement());
            }
            element.AddElement(control.Border.GetXMLElement());
            element.AddElement("Margins", "0,0,0,0");
            element.AddElement("Name", $"{name}{(id - 20)}");
            element.AddElement(new XElement("Page").AddAttribute("isRef", 15));
            element.AddElement(new XElement("Parent").AddAttribute("isRef", boundItem.BondType.ConvertToInt() + 1));
            string text = null;
            if (control.FieldData.SystemFiledType.HasValue)
                text = $"{{{control.FieldData.SystemFiledType}}}";
            if (control.FieldData.TotalFuncType.HasValue)
                text = $"{{{control.FieldData.TotalFuncType}}}";
            if (control.FieldData.SystemVariable.HasValue)
                text = $"{{{control.FieldData.SystemVariable}}}";
            if (control.FieldData.Path.HasValue())
            {
                var path = BusinessObject.GetBusinessObjectsPath(boundItem.BondType.ConvertToInt().Value - 2);
                text = $"{{{path}{control.FieldData.Path.Replace(".", "")}}}";
            }
            text ??= control.Text;
            if (control.ControlType == ControlType.TextBox) 
                element.AddElement("Text", text);
            else
            {
                element.AddElement("ImageBytes");
                if (control.ImageContent != null)
                    element.Element("ImageBytes").AddContent(Convert.ToBase64String(control.ImageContent));
                if (control.Stretch)
                    element.AddElement("Stretch", true);
            }
            element.AddElement("Type", "Expression");
            return element;
        }
    }
}
