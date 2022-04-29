using System;
using System.Xml;
using System.Text;
using System.Linq;
using Caspian.Common;
using System.Xml.Linq;
using Caspian.Common.Extension;
using System.Collections.Generic;

namespace ReportUiModels
{
    public class ReportPrintPage : ReportObject
    {
        public static int PageId { get; private set; }

        public static void ResetPageId()
        {
            ReportPrintPage.PageId = 0;
        }

        public ReportPrintPage(XElement element)
        {
            this.Border = new Border(element.Element("Border"));
            GuId = element.Element("Guid").Value;
            this.BackGroundColor = new Color(element.Element("Brush").Value);
            PageType = (ReportPageType)Convert.ToInt32(element.Element("PageType").Value);
            Width = Convert.ToDecimal(element.Element("PageWidth").Value);
            Height = Convert.ToDecimal(element.Element("PageHeight").Value);
            var bonds = element.Element("Components").Nodes().Where(t => t.NodeType != XmlNodeType.Text);
            //var test = bonds.Select(t => t.NodeType);
            Bonds = new List<Bond>();
            foreach (var item in bonds)
                Bonds.Add(new Bond(item as XElement));
        }

        public ReportPrintPage()
        {
            ReportPrintPage.PageId++;
        }

        public ReportPrint Report { get; set; }

        public int ReportId { get; set; }

        public decimal Height { get; set; }

        public decimal Width { get; set; }

        public decimal LeftMargin { get; set; }

        public decimal RightMargin { get; set; }

        public decimal TopMargin { get; set; }

        public decimal BottmMargin { get; set; }

        public Border Border { get; set; }

        public Color BackGroundColor { get; set; }

        public ReportPageType PageType { get; set; }

        public bool IsSubReport { get; set; }

        public Color Color { get; set; }

        public Font Font { get; set; }

        public string GuId { get; set; }

        public IList<Bond> Bonds { get; set; }

        public override XElement GetXmlElement(ReportType reportType)
        {
            string pageElementName = "Page";
            if (IsSubReport)
                pageElementName = "subReport_";
            var page = new XElement(pageElementName + PageId).AddAttribute("Ref", Id).AddAttribute("type", "Page").AddAttribute("isKey", true);
            page.Add(Border.GetXmlElement());
            var backGroundColor = new XElement("Brush");
            if (BackGroundColor == null)
                backGroundColor.Add("Transparent");
            else
                backGroundColor.Add(BackGroundColor.GetXmlElementValue());
            page.Add(backGroundColor);
            page.AddElement("Components").AddAttribute("isList", true).AddAttribute("count", Bonds.Count);
            for (var index = 0; index < Bonds.Count; index++)
            {
                var bond = Bonds[index];
                bond.Page = this;
                if (bond.BondType == BondType.DataHeader)
                {
                    var next = Bonds[index + 1];
                    if (next.ColumnsCount.HasValue && next.ColumnsCount.Value != ColumnCountType.Once)
                        bond.IsColumnBond = true;
                }
                else
                    if (bond.BondType == BondType.DataFooter)
                    {
                        var prev = Bonds[index - 1];
                        if (prev.ColumnsCount.HasValue && prev.ColumnsCount.Value != ColumnCountType.Once)
                            bond.IsColumnBond = true;
                    }
                bond.Height -= 0.024M;
                page.Element("Components").Add(bond.GetXmlElement(reportType));
            }
            page.AddElement("Conditions").AddAttribute("isList", true).AddAttribute("count", 0);
            page.AddElement("Guid").AddContent(GuId);
            LeftMargin = 1;
            RightMargin = 1;
            TopMargin = 1;
            BottmMargin = 1;
            page.AddElement("Margins").AddContent(0 + "," + 0 + "," + 0 + "," + 0);
            page.AddElement("Name").AddContent(pageElementName + ReportPrintPage.PageId);
            page.AddElement("PageType").AddContent(PageType.ConvertToInt());
            page.AddElement("PageWidth").AddContent(Width);
            page.AddElement("PageHeight").AddContent(Height);
            page.AddElement("Report").AddAttribute("isRef", 0);
            if (!IsSubReport)
            {
                page.AddElement("Watermark").AddAttribute("Ref", 9998).AddAttribute("type", "Stimulsoft.Report.Components.StiWatermark")
                    .AddAttribute("isKey", true);
                page.Element("Watermark").AddElement("Font").AddContent("Arial,100");
                page.Element("Watermark").AddElement("TextBrush").AddContent("[50:0:0:0]");
            }
            return page;
        }

        public override string GetJson()
        {
            var str = new StringBuilder("{" + "height:" + Height + ",width:" + Width + ",pageType:" + PageType.ConvertToInt());
            if (LeftMargin > 0)
                str.Append(",leftMargin:" + LeftMargin);
            if (IsSubReport)
                str.Append(",isSubReport:" + "true");
            if (RightMargin > 0)
                str.Append(",RightMargin:" + RightMargin);
            if (TopMargin > 0)
                str.Append(",topMargin:" + TopMargin);
            if (BottmMargin > 0)
                str.Append(",bottmMargin:" + BottmMargin);
            if (Border != null)
                str.Append(",border:" + Border.GetJson());
            if (BackGroundColor != null)
                str.Append(",backGroundColor:" + BackGroundColor.GetJson());
            if (Color != null)
                str.Append(",color:" + Color.GetJson());
            if (Font != null)
                str.Append(",font:" + Font.GetJson());
            if (Report != null)
                str.Append(",report:" + Report.GetJson());
            str.Append(",bonds:[");
            var isFirstTime = true;
            foreach (var bond in Bonds)
            {
                bond.Page = this;
                if (!isFirstTime)
                    str.Append(",");
                str.Append(bond.GetJson());
                isFirstTime = false;
            }
            str.Append("]");
            str.Append("}");
            return str.ToString();
        }
    }
}