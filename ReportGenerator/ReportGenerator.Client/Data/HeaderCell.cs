namespace Caspian.Report
{
    public class TableData
    {
        public IList<HeaderCell> HeaderCells { get; set; }

        public IList<TableRow> Rows { get; set; }

        public int Top { get; set; }

        public int TopStart { get; set; }
    }

    public class HeaderCell
    {
        public int Width { get; set; }
    }

    public class TableRow
    {
        public TableRow()
        {
            Cells = new List<TableCell>();
            Height = 30;
        }

        public int RowIndex { get; set; }

        public int Height { get; set; } = 30;

        public IList<TableCell> Cells { get; set; }
    }

    public class TableCell
    {
        public TableCell(TableRow row)
        {
            Row = row;
            RowSpan = 1;
            ColSpan = 1;
        }

        public int ColIndex { get; set; }

        public TableRow Row { get; set; }

        public string Text { get; set; }

        public NumberFormating NumberFormating { get; set; }

        public int RowSpan { get; set; }

        public int ColSpan { get; set; }

        public bool Hidden { get; set; }
    }
}
