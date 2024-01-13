using System.Text;
using Caspian.Common;
using System.Xml.Linq;

namespace Caspian.Common
{
    public class TableCell : ReportObject
    {
        public CellType CellType { get; set; }

        public string Member { get; set; }

        public TableCell()
        {
            JoinCells = new List<int>();
        }

        public int Width { get; set; }

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

        public override string GetJson()
        {
            throw new NotImplementedException();
        }
    }
}