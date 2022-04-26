using System;
using System.Xml;
using System.Linq;
using Caspian.Common;
using System.Xml.Linq;
using System.ComponentModel;
using Caspian.Common.Extension;
using System.Collections.Generic;

namespace ReportUiModels
{
    public class Bond : ReportObject
    {
        public Bond(XElement element)
        {
            var type = element.Attribute("type").Value;
            var isTable = false;
            switch(type)
            {
                case "ReportTitleBand":
                    BondType = BondType.ReportTitle;
                    break;
                case "PageHeaderBand":
                    BondType = BondType.PageHeader;
                    break;
                case "PageFooterBand":
                    BondType = BondType.PageFooter;
                    break;
                case "HeaderBand":
                case "Stimulsoft.Report.Components.StiColumnHeaderBand":
                    BondType = BondType.DataHeader;
                    break;
                case "DataBand":
                    BondType = BondType.DataBond;
                    break;
                case "FooterBand":
                    BondType = BondType.DataFooter;
                    break;
                case "Stimulsoft.Report.Components.Table.StiTable":
                    isTable = true;
                    BondType = BondType.DataBond;
                    break;
            }
            var columnsElement = element.Element("Columns");
            if (columnsElement != null)
                ColumnsCount = (ColumnCountType)Convert.ToInt32(columnsElement.Value);
            if (isTable)
            {
                Table = new TableControl(element);
                Height = Table.Cells.First().Position.Height;
            }
            else
            {
                var dataLevelElement = element.Element("DataLevel");
                if (dataLevelElement != null)
                    DataLevel = Convert.ToInt32(dataLevelElement.Value);
                PrintOn = GetPrintOn(element.Element("PrintOn"));
                BackGroundColor = new Color(element.Element("Brush").Value);
                var components = element.Element("Components").Nodes().Where(t => t.NodeType != XmlNodeType.Text);
                var position = new Position(element.Element("ClientRectangle"));
                if (BondType == BondType.DataBond)
                {
                    if (element.Element("NewPageBefore") != null)
                        NewPageBefore = Convert.ToBoolean(element.Element("NewPageBefore").Value.ToLower());
                    if (element.Element("NewPageAfter") != null)
                        NewPageAfter = Convert.ToBoolean(element.Element("NewPageAfter").Value.ToLower());
                }
                Height = position.Height;
                Controls = new List<ReportPrintControl>();
                foreach (var component in components)
                {
                    var temp = component as XElement;
                    var controlType = temp.Attribute("type").Value;
                    if (controlType == "Stimulsoft.Report.Components.Table.StiTable")
                        Table = new TableControl(temp);
                    else
                        Controls.Add(new ReportPrintControl(temp));
                }
            }
        }

        private PrintOnType GetPrintOn(XElement element)
        {
            if (element == null)
                return PrintOnType.AllPages;
            var value = element.Value;
            if (value == "ExceptFirstPage")
                return PrintOnType.ExceptFirstPage;
            if (value == "ExceptLastPage")
                return PrintOnType.ExceptLastPage;
            if (value == "ExceptFirstAndLastPage")
                return PrintOnType.ExceptFirstAndLastPage;
            throw new Exception("خطای عدم پیش بینی");
        }

        private string GetPrintOn(PrintOnType printOn)
        {
            switch(printOn)
            {
                case PrintOnType.AllPages:
                    return null;
                case PrintOnType.ExceptFirstPage:
                    return "ExceptFirstPage";
                case PrintOnType.ExceptLastPage:
                    return "ExceptLastPage";
                case PrintOnType.ExceptFirstAndLastPage:
                    return "ExceptFirstAndLastPage";
            }
            throw new Exception("خطای عدم پیش بینی");
        }

        public Bond() 
        {
            Controls = new List<ReportPrintControl>();
        }

        /// <summary>
        /// نوع چاپ سرصفحه نسبت به صفحات مختلف گزارش
        /// </summary>
        [DisplayName("نوع نمایش")]
        public PrintOnType? PrintOn { get; set; }

        [DisplayName("تعداد ستونها")]
        public ColumnCountType? ColumnsCount { get; set; }

        public int? MasterComponent { get; set; }

        public bool IsColumnBond { get; set; }

        [DisplayName("فاصله ی بین ستونها")]
        public int? ColumnsMargin { get; set; }

        public Border Border { get; set; }

        public ReportPrintPage Page { get; set; }

        /// <summary>
        /// صفحه جدید قبل از باند
        /// </summary>
        [DisplayName("صفحه جدید قبل از باند")]
        public bool NewPageBefore { get; set; }

        /// <summary>
        /// صفحه ی جدید بعد از باند
        /// </summary>
        [DisplayName("صفحه ی جدید بعد از باند")]
        public bool NewPageAfter { get; set; }

        public IList<ReportPrintControl> Controls { get; set; }

        public Color BackGroundColor { get; set; }

        public decimal Height { get; set; }

        public TableControl Table { get; set; }

        public BondType BondType { get; set; }

        public int? DataLevel { get; set; }

        public override XElement GetXmlElement(ReportType reportType)
        {
            XElement element = null;
            switch(BondType)
            {
                case BondType.ReportTitle:
                    if (Page.IsSubReport)
                        element = new XElement("ReportTitleBand2");
                    else
                        element = new XElement("ReportTitleBand1");
                    element.AddAttribute("Ref", Id).AddAttribute("type", "ReportTitleBand").AddAttribute("isKey", true);
                    break;
                case BondType.PageHeader:
                    element = new XElement("PageHeaderBand1").AddAttribute("Ref", Id).AddAttribute("type", "PageHeaderBand")
                        .AddAttribute("isKey", true);
                    break;
                case BondType.PageFooter:
                    element = new XElement("PageFooterBand1").AddAttribute("Ref", Id).AddAttribute("type", "PageFooterBand")
                        .AddAttribute("isKey", true);
                    break;
                case BondType.DataHeader:
                    if (IsColumnBond)
                        element = new XElement("ColumnHeaderBand1").AddAttribute("Ref", Id).AddAttribute("type", "Stimulsoft.Report.Components.StiColumnHeaderBand").AddAttribute("isKey", true);
                    else
                        element = new XElement("HeaderBand1").AddAttribute("Ref", Id).AddAttribute("type", "HeaderBand").AddAttribute("isKey", true);
                    break;
                case BondType.DataBond:
                    element = new XElement("DataBand" + DataLevel).AddAttribute("Ref", Id).AddAttribute("type", "DataBand")
                        .AddAttribute("isKey", true);
                    if (reportType == ReportType.View)
                        element.Add(new XElement("DataLevel", DataLevel));
                    element.Add(new XElement("NewPageBefore", NewPageBefore));
                    element.Add(new XElement("NewPageAfter", NewPageAfter));
                    break;
                case BondType.DataFooter:
                    if (IsColumnBond)
                        element = new XElement("ColumnFooterBand1").AddAttribute("Ref", Id).AddAttribute("type", "Stimulsoft.Report.Components.StiColumnFooterBand").AddAttribute("isKey", true);
                    else
                        element = new XElement("FooterBand1").AddAttribute("Ref", Id).AddAttribute("type", "FooterBand").AddAttribute("isKey", true);
                    break;
            }
            if (Border != null && Border.BorderKind != BorderKind.None)
                element.Add(Border.GetXmlElement());
            element.AddElement("Brush").AddContent(BackGroundColor == null ? "Transparent" : BackGroundColor.GetXmlElementValue());
            if (DataLevel > 0)
                element.AddElement("BusinessObjectGuid").AddContent(Page.Report.Dictionary.GetGuId(DataLevel.Value));
            if (DataLevel > 1)
            {
                Bond masterBond = null;
                foreach(var bond in Page.Bonds)
                {
                    if (bond.DataLevel == DataLevel + 1)
                        masterBond = bond;
                }
                if (masterBond != null)
                    element.AddElement("MasterComponent").AddAttribute("isRef", masterBond.Id);
            }
            var height = Height;
            if (reportType == ReportType.Report && Table != null)
            {
                if (BondType == BondType.DataHeader)
                {
                    var headerCellHeight = Table.Cells.First(t => t.CellType == CellType.ColumnHeader).Position.Height - 0.03m;
                    height -= headerCellHeight;
                }
                else
                    if (BondType == BondType.DataBond)
                        height = 0;
            }
            if (ColumnsCount.HasValue && ColumnsCount.Value != ColumnCountType.Once)
                element.AddElement("Columns", ColumnsCount.ConvertToInt());
            element.Add(new Position(0, 0, Page.Width, height).GetXmlElement());
            var count = Controls.Count;
            if (Table != null)
                if (reportType == ReportType.Report)
                    count += Table.Cells.Count(t => t.Enable);
                else
                    count++;

            element.AddElement("Components").AddAttribute("isList", true).AddAttribute("count", count);
            if (Table != null)
            {
                Table.Bond = this;
                if (reportType == ReportType.Report)
                {
                    foreach (var cell in Table.Cells.Where(t => t.Enable))
                        element.Element("Components").Add(new ReportPrintControl(cell).GetXmlElement(ReportType.Report));
                }
                else
                    element.Element("Components").Add(Table.GetXmlElement(reportType));
            }
            if (MasterComponent.HasValue)
                element.AddElement("MasterComponent").AddAttribute("isRef", MasterComponent.Value);
            foreach (var control in Controls)
            {
                control.Bond = this;
                element.Element("Components").Add(control.GetXmlElement(reportType));
            }
            element.AddElement("Conditions").AddAttribute("isList", true).AddAttribute("count", 0);
            element.AddElement("Name").AddContent(element.Name);
            element.AddElement("Page").AddAttribute("isRef", Page.Id);
            element.AddElement("Parent").AddAttribute("isRef", Page.Id);
            if (PrintOn.HasValue && PrintOn != PrintOnType.AllPages)
                element.AddElement("PrintOn", GetPrintOn(PrintOn.Value));
            return element;
        }

        public override string GetJson()
        {
            var str = "{height:" + Height + ",bondType:" + this.BondType.ConvertToInt();
            if (PrintOn.HasValue && PrintOn.Value != PrintOnType.AllPages)
                str += ",printOn:" + PrintOn.ConvertToInt();
            if (ColumnsCount.HasValue)
                str += ",columnsCount:" + ColumnsCount.Value.ConvertToInt();
            if (BackGroundColor != null)
                str += ",backGroundColor:" + BackGroundColor.GetJson();
            if (DataLevel.HasValue)
                str += ",dataLevel:" + DataLevel.Value;
            if (NewPageAfter)
                str += ",newPageAfter:true";
            if (NewPageBefore)
                str += ",newPageBefore:true";
            if (Table != null)
                str += ",table:" + Table.GetJson();
            else
            {
                str += ",controls:[";
                var isFirstTime = true;
                foreach (var control in Controls)
                {
                    control.Bond = this;
                    if (!isFirstTime)
                        str += ',';
                    str += control.GetJson();
                    isFirstTime = false;
                }
                str += "]";
            }
            str += "}";
            return str;
        }
    }


}