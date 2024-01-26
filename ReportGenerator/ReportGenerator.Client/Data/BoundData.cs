namespace Caspian.Report.Data
{
    public class BoundData
    {
        public byte DataLevel { get; set; }

        public byte ColumnCount { get; set; }

        public byte ColumnGap { get; set; }

        public IList<BoundItemData> Items { get; set; }
    }
}
