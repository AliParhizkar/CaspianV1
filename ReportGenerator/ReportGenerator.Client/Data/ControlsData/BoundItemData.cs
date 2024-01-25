﻿namespace Caspian.Report.Data
{
    public class BoundItemData
    {
        public BoundItemData()
        {
            Controls = new List<ControlData>();
        }

        public TableData Table { get; set; }

        public IList<ControlData> Controls { get; set; }

        public int Height { get; set; }

        public BondType BondType { get; set; }

        public byte ColumnsCount { get; set; }

        public byte GapBetweenColumns { get; set; }
    }
}
