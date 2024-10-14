namespace Caspian.Report.Data
{
    public class ControlData
    {
        public ControlData(double left, double top, ControlType controlType) 
        {
            Left = left;
            Top = top;
            this.ControlType = controlType;
            Alignment = new Alignment();
            if (controlType == ControlType.TextBox)
                Font = new Font();
            Border = new Border();
            FieldData = new TextFieldData();
            NumberFormating = new NumberFormating();
            BackgroundColor = new Color()
            {
                ColorString = "transparent"
            };
        }

        public string Id { get; set; }

        public ControlType ControlType { get; private set; }

        public double Left { get; set; }

        public double Width { get; set; }

        public double Height { get; set; }

        public double Top { get; set; }

        public string Text { get; set; }

        public byte[] ImageContent { get; set; }

        public bool Stretch { get; set; }

        public BondType? BondType { get; set; }

        public Color BackgroundColor { get; set; }

        public Alignment Alignment { get; set; }

        public Font Font { get; set; }

        public TextFieldData FieldData { get; set; }

        public NumberFormating NumberFormating { get; set; }

        public Border Border { get; set; }
    }
}
