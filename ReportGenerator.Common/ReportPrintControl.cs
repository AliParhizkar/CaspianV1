using System.Text;
using System.Xml.Linq;

namespace Caspian.Common
{
    public class ReportPrintControl : ReportObject
    {
        public Bond Bond { get; set; }

      

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

        public override string GetJson()
        {
            throw new NotImplementedException();
        }
    }
}