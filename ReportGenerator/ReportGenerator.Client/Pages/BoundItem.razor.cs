using Caspian.Common;
using Caspian.Report.Data;
using Microsoft.AspNetCore.Components;

namespace Caspian.Report
{
    public partial class BoundItem
    {
        public Table Table { get; private set; }

        public int Top { get { return GetTopBounnd(Data.BondType); } }

        public int Bottom { get { return Top + Data.Height; } }

        public IList<ReportControl> ReportControls { get; private set; }

        protected override void OnInitialized()
        {
            ReportControls = new List<ReportControl>();
            base.OnInitialized();
        }

        ReportControl ReportControl {get { return default; }set { ReportControls.Add(value); }}

        [Parameter]
        public Bound Bound { get; set; }

        [Parameter]
        public BoundItemData Data { get; set; }

        public void RemoveSelectedItem()
        {
            if (Bound.Page.SelectedControl != null)
                Data.Controls.Remove(Bound.Page.SelectedControl.Data);
            else
                Data.Table = null;
        }

        int ColumnWidth
        {
            get
            {
                var width = Bound.Page.Data.Width;
                if (Data.ColumnsCount == 0)
                    return width;
                return (width - Data.GapBetweenColumns * (Data.ColumnsCount - 1)) / Data.ColumnsCount;
            }
        }

        public int GetTopBounnd(BondType bondType)
        {
            var top = Bound.Top;
            foreach (var bound in Bound.Data.Items)
            {
                if (bound.BondType == bondType)
                    break;
                top += bound.Height + Bound.BoundBetweenSpace;
            }
            return top;
        }

        public void GetRecDatas(IList<RecData> list)
        {
            if (Table != null && Table != Bound.Page.SelectedTable)
            {
                var sumLeft = Table.Data.Left + 15;
                foreach (var cell in Table.Data.HeaderCells)
                {
                    list.Add(new RecData()
                    {
                        Left = sumLeft,
                        Width = cell.Width,
                    });
                    sumLeft += cell.Width;
                }
            }
            var result = ReportControls.Where(t => t != Bound.Page.SelectedControl).Select(t => new RecData()
            {
                Left = t.Data.Left,
                Top = t.Data.Top,
                Width = t.Data.Width,
                Height = t.Data.Height
            }).ToArray();
            list.AddRange(result);
        }

        public void SetTopStartForControlsAndTable()
        {
            if (Table != null)
                Table.TopStart = Table.Data.Top;
            foreach(var control in ReportControls)
                control.TopStart = control.Data.Top;
        }

        /// <summary>
        /// This method update top of controls and table on bound dragging
        /// </summary>
        public void UpdateTopOnBoundDrag(int difY)
        {
            if (Table != null)
                Table.Data.Top = Table.TopStart + difY;
            foreach (var control in ReportControls)
                control.Data.Top = control.TopStart + difY;
        }

        public string Title
        {
            get
            {
                return Data.BondType.EnumText();
            }
        }

        public byte? DataLevel
        {
            get
            {
                if (Data.BondType < BondType.FirstDataLevel || Data.BondType > BondType.ThirdDataLevel)
                    return null;
                return (byte)((Data.BondType - BondType.FirstDataLevel) + 1);
            }
        }
    }
}
