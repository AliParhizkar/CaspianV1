using System;
using System.Text;
using System.Xml.Linq;
using System.Collections.Generic;
using Caspian.Common;
using Caspian.Common.Extension;

namespace ReportUiModels
{
    public class TableCell : ReportObject
    {
        public TableCell(XElement element)
        {
            if (element.Element("Brush") != null)
                BackGroundColor = new Color(element.Element("Brush").Value);
            if (element.Element("StringFormat") != null)
                Format = new Format(element.Element("StringFormat"));
            JoinWidth = 1;
            if (element.Element("JoinWidth") != null)
                JoinWidth = Convert.ToInt32(element.Element("JoinWidth").Value);
            if (element.Element("TextBrush") != null)
                Color = new Color(element.Element("TextBrush").Value);
            JoinHeight = 1;
            if (element.Element("JoinHeight") != null)
                JoinHeight = Convert.ToInt32(element.Element("JoinHeight").Value);
            var align = element.Element("VertAlignment");
            if (align != null)
                VerticalAlign = (VerticalAlign)typeof(VerticalAlign).GetField(align.Value).GetValue(null);
            align = element.Element("HorAlignment");
            if (align != null)
                HorizontalAlign = (HorizontalAlign)typeof(HorizontalAlign).GetField(align.Value).GetValue(null);
            Enable = true;
            if (element.Element("Font") != null)
                Font = new Font(element.Element("Font"));
            if (element.Element("Border") != null)
                Border = new Border(element.Element("Border"));
            if (element.Element("Enabled") != null)
                Enable = Convert.ToBoolean(element.Element("Enabled").Value);
            if (element.Element("FaText") != null)
                Text = element.Element("FaText").Value;
            if (element.Element("Text") != null)
                Member = element.Element("Text").Value;
            Position = new Position(element.Element("ClientRectangle"));
        }

        

        public CellType CellType { get; set; }

        public string Member { get; set; }

        public TableCell()
        {
            JoinCells = new List<int>();
        }

        public TableControl Table { get; set; }

        public int? RowSpan { get; set; }

        public int? JoinHeight { get; set; }

        public int? JoinWidth { get; set; }

        public int? ColSpan { get; set; }

        public Border Border { get; set; }

        public string Text { get; set; }

        public Position Position { get; set; }

        public Font Font { get; set; }

        public Color BackGroundColor { get; set; }

        public HorizontalAlign? HorizontalAlign { get; set; }

        public VerticalAlign? VerticalAlign { get; set; }

        public Color Color { get; set; }

        public IList<int> JoinCells { get; private set; }

        public int? ParentJoin { get; set; }

        public Format Format { get; set; }

        public bool Enable { get; set; }

        public override XElement GetXmlElement(ReportType reportType)
        {
            var element = new XElement("Table" + Table.Id + "_Cell" + Id).AddAttribute("Ref", Id)
                .AddAttribute("type", "Stimulsoft.Report.Components.Table.StiTableCell").AddAttribute("isKey", true);
            if (CellType == CellType.Cell)
            {
                element.Add(Border.GetXmlElement());
                element.AddElement("Brush").AddContent(BackGroundColor.GetXmlElementValue());
            }
            element.Add(Position.GetXmlElement());
            element.AddElement("Conditions").AddAttribute("isList", true).AddAttribute("count", 0);
            element.AddElement("ID").AddContent(Id);
            if (CellType == CellType.Cell)
            {
                element.Add(Font.GetXmlElement());
                var align = HorizontalAlign;
                if (reportType == ReportType.Report)
                {
                    ///بخاطر برعکس بودن راست به چپ جدول در کامپوننت
                    if (align == ReportUiModels.HorizontalAlign.Left)
                        align = ReportUiModels.HorizontalAlign.Right;
                    else
                        if (align == ReportUiModels.HorizontalAlign.Right)
                            align = ReportUiModels.HorizontalAlign.Left;
                }
                if (align.HasValue)
                    element.AddElement("HorAlignment").AddContent(align.ToString());
                if (VerticalAlign.HasValue)
                    element.AddElement("VertAlignment").AddContent(VerticalAlign);
                if (JoinCells.Count > 0)
                    element.AddElement("Join").AddContent(true);
                element.AddElement("JoinCells").AddAttribute("isList", true).AddAttribute("count", JoinCells.Count);
                foreach (var item in JoinCells)
                    element.Element("JoinCells").AddElement("value").AddContent(item);
                if (ParentJoin.HasValue)
                    element.AddElement("ParentJoin").AddContent(ParentJoin.Value);
                if (align == ReportUiModels.HorizontalAlign.Left)
                    element.AddElement("Margins").AddContent("0,1,0,0");
                else
                    element.AddElement("Margins").AddContent("0,0,0,0");
                element.AddElement("Name").AddContent("Table" + Table.Id + "_Cell" + Id);
                if (!Enable)
                    element.AddElement("Enabled").AddContent(false);
                if (JoinWidth.GetValueOrDefault(1) > 1)
                    element.AddElement("JoinWidth").AddContent(JoinWidth.Value);
                if (JoinHeight.GetValueOrDefault(1) > 1)
                    element.AddElement("JoinHeight").AddContent(JoinHeight.Value);
                if (Table.Bond == null)
                    element.AddElement("Page").AddAttribute("isRef", Table.Page.Id);
                else
                    element.AddElement("Page").AddAttribute("isRef", Table.Bond.Page.Id);
                element.AddElement("Parent").AddAttribute("isRef", Table.Id);
                element.AddElement("Restrictions").AddContent("AllowMove, AllowSelect, AllowChange");
                if (Member == null)
                    element.AddElement("Text").AddContent(Text ?? "");
                else
                {
                    string member = Member; ;
                    if (Format != null && reportType == ReportType.Report)
                    {
                        var formatString = Format.GetFormatString(Member);
                        if (formatString.HasValue())
                            member = formatString;
                    }
                    element.AddElement("Text").AddContent(member);
                }
            }
            if (CellType == ReportUiModels.CellType.Cell)
                element.AddElement("TextBrush").AddContent(Color.GetXmlElementValue());
            if (reportType == ReportType.View)
            {
                element.AddElement("CellType").AddContent(CellType);
                element.AddElement("FaText").AddContent(Text ?? "");

                ///ذخیره ی فرمت در فایل
                if (Format != null)
                    element.Add(Format.GetXml());
            }
            else
                element.AddElement("TextOptions").AddContent("HotkeyPrefix=None, LineLimit=False, RightToLeft=True, Trimming=None, WordWrap=True, " +
                    "Angle=0, FirstTabOffset=40, DistanceBetweenTabs=20");
            element.AddElement("Type").AddContent("Expression");
            return element;
        }

        public override string GetJson()
        {
            var str = new StringBuilder("{cellType:" + CellType.ConvertToInt());
            if (Member.HasValue())
                str.Append(",member:\"" + System.Uri.EscapeDataString(Member) + "\"");
            if (RowSpan.HasValue)
                str.Append(",rowSpan:" + RowSpan.Value);
            if (JoinHeight.HasValue)
                str.Append(",joinHeight:" + JoinHeight.Value);
            if (JoinWidth.HasValue)
                str.Append(",joinWidth:" + JoinWidth.Value);
            if (ColSpan.HasValue)
                str.Append(",colSpan:" + ColSpan.Value);
            if (Border != null)
                str.Append(",border:" + Border.GetJson());
            if (Text.HasValue())
                str.Append(",text:\"" + System.Uri.EscapeDataString(Text) + "\"");
            if (Position != null)
                str.Append(",position:" + Position.GetJson());
            if (Font != null)
                str.Append(",font:" + Font.GetJson());
            if (Format != null)
                str.Append(",format:" + Format.GetJson());
            if (BackGroundColor != null)
                str.Append(",backGroundColor:" + BackGroundColor.GetJson());
            if (HorizontalAlign.HasValue)
                str.Append(",horizontalAlign:" + HorizontalAlign.ConvertToInt());
            if (VerticalAlign.HasValue)
                str.Append(",verticalAlign:" + VerticalAlign.ConvertToInt());
            if (Color != null)
                str.Append(",color:" + Color.GetJson());
            if (Enable)
                str.Append(",enable:true");
            str.Append("}");
            return str.ToString();
        }
    }
}