
namespace Caspian.Common
{
    public class TableControl : ReportObject
    {
        public int? DataLevel { get; set; }

        public Bond Bond { get; set; }

        public ReportPrintPage Page { get; set; }

        public Position Position { get; set; }

        public int RowsCount { get; set; }

        public int ColumnsCount { get; set; }

        public IList<TableCell> Cells { get; set; }

        public override string GetJson()
        {
            throw new NotImplementedException();
        }
    }
}