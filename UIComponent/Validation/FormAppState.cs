namespace Caspian.UI
{
    public class FormAppState
    {
        public bool AllControlsIsValid { get; set; }

        public string ErrorMessage { get; set; }

        public IControl Control { get; set; }

        public bool ValidationChecking { get; set; }
    }
}
