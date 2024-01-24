namespace Caspian.Report.Data
{
    public class TableData
    {
        public IList<HeaderCellData> HeaderCells { get; set; }

        public IList<TableRowData> Rows { get; set; }

        public int Left { get; set; }

        public int Top { get; set; }

        public BondType? BondType { get; set; }
    }
}
