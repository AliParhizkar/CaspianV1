namespace Caspian.Report
{
    public class HeaderCell
    {
        public int Width { get; set; }

        public int Index { get; set; }
    }

    public class TableRow
    {
        public TableRow()
        {
            Cells = new List<TableCell>();
        }

        public int Height { get; set; } = 50;

        public IList<TableCell> Cells { get; set; }
    }

    public class TableCell
    {
        public string Text { get; set; }

        public int RowSpan { get; set; } = 1;

        public int ColSpan { get; set; } = 1;

        public bool Visible { get; set; }
    }
}
