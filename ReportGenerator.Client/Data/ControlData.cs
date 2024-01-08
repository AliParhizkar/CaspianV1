namespace Caspian.Report
{
    public class ControlData
    {
        public ControlData(double left, double top) 
        {
            Left = left;
            Top = top;
        }

        public double Left { get; set; }

        public double Top { get; set; }

        public BondType? BondType { get; set; }
    }
}
