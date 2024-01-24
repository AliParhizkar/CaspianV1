namespace Caspian.Report.Data
{
    public class TableCellData
    {
        public TableCellData(TableRowData row)
        {
            Row = row;
            RowSpan = 1;
            ColSpan = 1;
        }

        public int ColIndex { get; set; }

        public TableRowData Row { get; set; }

        public string Text { get; set; }

        public NumberFormating NumberFormating { get; set; }

        public int RowSpan { get; set; }

        public int ColSpan { get; set; }

        public bool Hidden { get; set; }
    }
}
