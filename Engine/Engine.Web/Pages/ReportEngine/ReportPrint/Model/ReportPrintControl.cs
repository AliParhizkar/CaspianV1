using System.Text;
using Caspian.Common;
using System.Xml.Linq;
using Caspian.Common.Extension;

namespace ReportUiModels
{
    public class ReportPrintControl : ReportObject
    {
        public Bond Bond { get; set; }

        public ReportPrintControl(XElement element)
        {
            var type = element.Attribute("type").Value;
            if (element.Element("SystemFiledType")?.Value.HasValue() == true)
                SystemFiledType = (SystemFiledType)Convert.ToInt32(element.Element("SystemFiledType").Value);
            if (element.Element("SystemVariable")?.Value.HasValue() == true)
                SystemVariable = (SystemVariable)Convert.ToInt32(element.Element("SystemVariable").Value);
            switch (type)
            {
                case "Text":
                    this.Type = ReportControlType.TextBox;
                    Color = new Color(element.Element("TextBrush").Value);
                    Font = new Font(element.Element("Font"));
                    Member = element.Element("Text").Value;
                    Text = element.Element("FaText").Value;
                    if (element.Element("StringFormat") != null)
                        Format = new Format(element.Element("StringFormat"));
                    break;
                case "Image":
                    this.Type = ReportControlType.PictureBox;
                    ImageFileName = element.Element("File")?.Value;
                    if (element.Element("Text") != null)
                        Text = element.Element("Text").Value;
                    Member = element.Element("DataColumn").Value;
                    Stretch = Convert.ToBoolean(element.Element("Stretch").Value);
                    break;
                case "SubReport":
                    this.Type = ReportControlType.SubReport;
                    this.Guid = element.Element("SubReportPageGuid").Value;
                    break;
            }

            if (Type != ReportControlType.SubReport)
            {
                var border = element.Element("Border");
                if (border != null)
                    Border = new Border(border);
                var align = element.Element("HorAlignment");
                if (align != null)
                    HorizontalAlign = (HorizontalAlign)typeof(HorizontalAlign).GetField(align.Value).GetValue(null);
                align = element.Element("VertAlignment");
                if (align != null)
                    VerticalAlign = (VerticalAlign)typeof(VerticalAlign).GetField(align.Value).GetValue(null);
            }
            BackGroundColor = new Color(element.Element("Brush").Value);
            Position = new Position(element.Element("ClientRectangle"));
        }

        public ReportPrintControl()
        {
            Border = new Border();
            Position = new Position();
        }

        public ReportPrintControl(TableCell cell)
        {
            BackGroundColor = cell.BackGroundColor;
            Border = cell.Border;
            Color = cell.Color;
            Font = cell.Font;
            Format = cell.Format;
            HorizontalAlign = cell.HorizontalAlign;
            Member = cell.Member;
            if (cell.Position != null)
                Position = new Position(cell.Position.Left, cell.Position.Top - 0.529M, cell.Position.Width, cell.Position.Height);
            Text = cell.Text;
            VerticalAlign = cell.VerticalAlign;
            Type = ReportControlType.TextBox;
            Bond = cell.Table.Bond;

        }

        public SystemFiledType? SystemFiledType { get; set; }

        public SystemVariable? SystemVariable { get; set; }

        public ReportControlType Type { get; set; }

        public Border Border { get; set; }

        public string Member { get; set; }

        public Color BackGroundColor { get; set; }

        public Position Position { get; set; }

        public HorizontalAlign? HorizontalAlign { get; set; }

        public VerticalAlign? VerticalAlign { get; set; }

        public string Text { get; set; }

        public Color Color { get; set; }

        public Font Font { get; set; }

        public string ImageFileName { get; set; }

        public bool Stretch { get; set; }

        public Format Format { get; set; }

        public ReportPrintPage SubReportPage { get; set; }

        public string Guid { get; set; }

        public override XElement GetXmlElement(ReportType reportType)
        {
            var name = "";
            switch (Type)
            {
                case ReportControlType.TextBox:
                    name = "Text";
                    break;
                case ReportControlType.PictureBox:
                    name = "Image";
                    break;
                case ReportControlType.SubReport:
                    name = "SubReport";
                    break;
                default: throw new Exception("خطا:Value " + Type + " is invalid");
            }
            var element = new XElement(name + Id).AddAttribute("Ref", Id).AddAttribute("type", name).AddAttribute("isKey", true);
            if (Type == ReportControlType.TextBox)
                element.Add(Font.GetXmlElement());
            var color = "Transparent";

            if (BackGroundColor != null)
                color = BackGroundColor.GetXmlElementValue();
            element.AddElement("Brush").AddContent(color);
            if (this.Type == ReportControlType.PictureBox)
                element.AddElement("Stretch").AddContent(Stretch);
            if (Type == ReportControlType.SubReport)
                element.AddElement("SubReportPageGuid").AddContent(this.Guid);
            else
            {
                if (Border.BorderKind != BorderKind.None)
                    element.Add(Border.GetXmlElement());
                element.AddElement("Conditions").AddAttribute("isList", true).AddAttribute("count", 0);
                if (HorizontalAlign.HasValue)
                {
                    var align = HorizontalAlign;
                    if (reportType == ReportType.Report)
                    {
                        element.AddElement("TextOptions").AddContent("HotkeyPrefix=None, LineLimit=False, RightToLeft=True, Trimming=None, WordWrap=True, " +
                            "Angle=0, FirstTabOffset=40, DistanceBetweenTabs=20");
                        ///بخاطر برعکس بودن راست به چپ جدول در کامپوننت
                        if (align == ReportUiModels.HorizontalAlign.Left)
                            align = ReportUiModels.HorizontalAlign.Right;
                        else
                            if (align == ReportUiModels.HorizontalAlign.Right)
                                align = ReportUiModels.HorizontalAlign.Left;
                    }
                    else
                    {
                        element.AddElement("SystemFiledType", SystemFiledType.ConvertToInt());
                        element.AddElement("SystemVariable", SystemVariable.ConvertToInt());
                    }
                    element.AddElement("HorAlignment").AddContent(align.ToString());
                }
                if (VerticalAlign.HasValue)
                    element.AddElement("VertAlignment").AddContent(VerticalAlign.ToString());
                element.AddElement("Margins").AddContent("0,0,0,0");
            }
            element.Add(Position.GetXmlElement());
            element.AddElement("Page").AddAttribute("isRef", Bond.Page.Id);
            element.AddElement("Parent").AddAttribute("isRef", Bond.Id);
            switch (Type)
            {
                case ReportControlType.TextBox:
                    if (reportType == ReportType.View)
                    {
                        element.AddElement("FaText").AddContent(Text);
                        if (Format != null)
                            element.Add(Format.GetXml());
                    }
                    element.AddElement("Name").AddContent("Text" + Id);
                    if (Member == null)
                        element.AddElement("Text").AddContent(Text);
                    else
                    {
                        string member = Member;
                        if (Format != null && reportType == ReportType.Report)
                        {
                            var formatString = Format.GetFormatString(Member);
                            if (formatString.HasValue())
                                member = formatString;
                        }
                        element.AddElement("Text").AddContent(member);
                    }
                    element.AddElement("TextBrush").AddContent(Color.GetXmlElementValue());
                    element.AddElement("Type").AddContent("Expression");
                    break;
                case ReportControlType.PictureBox:
                    element.AddElement("Name").AddContent("Image" + Id);
                    if (ImageFileName.HasValue())
                        element.AddElement("File").AddContent(ImageFileName);
                    else
                    {
                        if (reportType == ReportType.View && Text.HasValue())
                            element.AddElement("Text").AddContent(Text);
                    }
                    element.AddElement("DataColumn").AddContent(Member);
                    element.AddElement("Type").AddContent("Expression");
                    break;
                case ReportControlType.SubReport:
                    element.AddElement("Name").AddContent("SubReport" + Id);
                    var pages = Bond.Page.Report.Pages;
                    foreach (var page in pages)
                        if (page.GuId == Guid)
                            foreach (var bond in page.Bonds)
                                if (bond.BondType == BondType.DataBond)
                                    bond.MasterComponent = Bond.Id;
                    break;
            }
            return element;
        }

        public override string GetJson()
        {
            var str = new StringBuilder();
            str.Append("{type:" + Type.ConvertToInt());
            if (Border != null)
                str.Append(",border:" + Border.GetJson());
            
            if (Member.HasValue())
                str.Append(",member:\"" +  System.Uri.EscapeDataString(Member) + "\"");
            if (BackGroundColor != null)
                str.Append(",backGroundColor:" + BackGroundColor.GetJson());
            if (Position != null)
                str.Append(",position:" + Position.GetJson());
            if (HorizontalAlign.HasValue)
                str.Append(",horizontalAlign:" + HorizontalAlign.ConvertToInt());
            if (VerticalAlign.HasValue)
                str.Append(",verticalAlign:" + VerticalAlign.ConvertToInt());
            if (Text.HasValue())
                str.Append(",text:\"" + System.Uri.EscapeDataString(Text) + "\"");
            if (Color != null)
                str.Append(",color:" + Color.GetJson());
            if (Font != null)
                str.Append(",font:" + Font.GetJson());
            if (Format != null)
                str.Append(",format:" + Format.GetJson());
            if (SystemFiledType.HasValue)
                str.Append(",systemFiledType:" + SystemFiledType.ConvertToInt());
            if (SystemVariable.HasValue)
                str.Append(",systemVariable:" + SystemVariable.ConvertToInt());
            if (Guid.HasValue())
            {
                foreach (var page in Bond.Page.Report.Pages)
                    if (page.GuId == Guid)
                        str.Append(",subReport:" + page.GetJson());
            }
            if (ImageFileName.HasValue())
            {
                var array = File.ReadAllBytes(ImageFileName);
                var data = Convert.ToBase64String(array);
                str.Append(",uri:\"" + data + "\"");
                var fileName = Path.GetFileName(ImageFileName);
                str.Append(",imageFileName:\"" + fileName + "\"");
            }
            if (Stretch)
                str.Append(",stretch:true");
            if (Guid.HasValue())
                str.Append(",guid:\"" + Guid + "\"");
            str.Append("}");
            return str.ToString();
        }
    }
}