namespace Caspian.Common
{
    public class Bond : ReportObject
    {

        public Bond() 
        {
            Controls = new List<ReportPrintControl>();
        }


        public PrintOnType? PrintOn { get; set; }

        public ColumnCountType? ColumnsCount { get; set; }

        public int? MasterComponent { get; set; }

        public bool IsColumnBond { get; set; }

        public int? ColumnsMargin { get; set; }

        public Border Border { get; set; }

        public ReportPrintPage Page { get; set; }

        public bool NewPageBefore { get; set; }

        public bool NewPageAfter { get; set; }

        public IList<ReportPrintControl> Controls { get; set; }

        public Color BackGroundColor { get; set; }

        public decimal Height { get; set; }

        public TableControl Table { get; set; }

        public BondType BondType { get; set; }

        public int? DataLevel { get; set; }

        public override string GetJson()
        {
            throw new NotImplementedException();
        }
    }


}