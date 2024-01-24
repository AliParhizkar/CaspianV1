namespace Caspian.Report.Data
{
    public class ControlData
    {
        public ControlData(double left, double top) 
        {
            Left = left;
            Top = top;
        }

        public double Left { get; set; }

        public double Width { get; set; }

        public double Height { get; set; }

        public double Top { get; set; }

        public BondType? BondType { get; set; }
    }
}
