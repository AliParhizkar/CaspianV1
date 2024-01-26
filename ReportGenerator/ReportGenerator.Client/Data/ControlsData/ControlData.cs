namespace Caspian.Report.Data
{
    public class ControlData
    {
        public ControlData(double left, double top) 
        {
            Left = left;
            Top = top;
            Alignment = new Alignment();
            Font = new Font();
            Border = new Border();
            TextFieldData = new TextFieldData();
            NumberFormating = new NumberFormating();
        }

        public double Left { get; set; }

        public double Width { get; set; }

        public double Height { get; set; }

        public double Top { get; set; }

        public string Text { get; set; }

        public BondType? BondType { get; set; }

        public Alignment Alignment { get; set; }

        public Font Font { get; set; }

        public TextFieldData TextFieldData { get; set; }

        public NumberFormating NumberFormating { get; set; }

        public Border Border { get; set; }
    }
}
