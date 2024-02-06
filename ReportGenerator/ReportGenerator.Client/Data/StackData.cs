using Caspian.Report.Data;

namespace ReportGenerator.Client.Data
{
    public class StackData
    {
        public string Id {  get; set; }

        public ControlData Control { get; set; }

        public BoundItemData BoundItem{ get; set; }

        public BoundData Bound {  get; set; }
    }
}
