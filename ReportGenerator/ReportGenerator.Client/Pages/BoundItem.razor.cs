using Caspian.Common;
using Caspian.Report.Data;
using Microsoft.AspNetCore.Components;

namespace Caspian.Report
{
    public partial class BoundItem
    {
        Table table;

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
            if (table != null && table != Bound.Page.SelectedTable)
            {
                var sumLeft = table.Data.Left + 15;
                foreach (var cell in table.Data.HeaderCells)
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
