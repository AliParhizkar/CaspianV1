﻿using System.Text.Json.Serialization;

namespace Caspian.Report.Data
{
    public class TableCellData
    {
        public TableCellData(TableRowData row)
        {
            Row = row;
            RowSpan = 1;
            ColSpan = 1;
            Alignment = new Alignment();
            Font = new Font();
            Border = new Border()
            {
                BorderKind = BorderKind.Bottom | BorderKind.Right,
                Color = new Color() { ColorString = "#808080" }
            };
        }

        public int ColIndex { get; set; }

        [JsonIgnore]
        public TableRowData Row { get; set; }

        public string Text { get; set; }

        public NumberFormating NumberFormating { get; set; }

        public int RowSpan { get; set; }

        public int ColSpan { get; set; }

        public bool Hidden { get; set; }

        public Alignment Alignment { get; set; }

        public Font Font { get; set; }

        public Border Border { get; set; }
    }
}
