namespace Caspian.Report.Data
{
    public class TableData
    {
        public TableData() 
        {
            HeaderCells = new List<HeaderCellData>();
            Rows = new List<TableRowData>();
            Border = new Border();
        }

        public IList<HeaderCellData> HeaderCells { get; set; }

        public IList<TableRowData> Rows { get; set; }

        public int Left { get; set; }

        public int Top { get; set; }

        public BondType? BondType { get; set; }

        public Border Border { get; set; }  

    }
}
