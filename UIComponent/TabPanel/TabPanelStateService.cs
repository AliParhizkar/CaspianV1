using System.Collections.Generic;

namespace Caspian.UI
{
    public class TabPanelStateService
    {
        public TabPanelStateService()
        {
            Data = new List<TabPanelStateData>();
        }

        public int Index { get; set; }

        public TabPanelState TabPanelState { get; set; }

        public List<TabPanelStateData> Data { get; set; }

        public int SelectedIndex { get; set; }

        public bool LoadOnDemand { get; set; }
    }
}
