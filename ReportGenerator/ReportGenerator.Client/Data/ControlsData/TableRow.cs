namespace Caspian.Report.Data
{
    public class TableRowData
    {
        public TableRowData()
        {
            Cells = new List<TableCellData>();
            Height = 30;
        }

        public int RowIndex { get; set; }

        public int Height { get; set; } = 30;

        public IList<TableCellData> Cells { get; set; }
    }
}
