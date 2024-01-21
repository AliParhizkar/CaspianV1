namespace Caspian.Report
{
    public class RecData
    {
        public BondType BondType 
        {
            get
            {
                return (BondType)BondValue;
            }
        }

        public string Id { get; set; }

        public int BondValue { get; set; }

        public double Width { get; set; }

        public double Height { get; set; }

        public double Top { get; set; }

        public double Bottom { get; set; }

        public double Left { get; set; }

        public double Right { get; set; }
    }
}
