using System.Data;
using Caspian.Report.Data;
using Caspian.Common.Extension;
using Microsoft.AspNetCore.Components;

namespace Caspian.Report
{
    public partial class Bound: ComponentBase
    {
        public const int Left = 182;
        public const int Top = 53;
        public const int BoundBetweenSpace = 3;
        IList<BoundItem> bounditems; 

        BoundItem BoundItem { get { return default; } set { bounditems.Add(value); } }
        
        double? horizontalRulerTop, horizontalRulerBottom, verticalRulerLeft, verticalRulerRight;
        
        bool canChangeHeight;
        double yStart, heightStart;

        public int Right
        {
            get
            {
                return Left + Page.Data.Width;
            }
        }

        public bool SelectionIsDisabled { get; private set; }

        [Parameter]
        public BoundData Data { get; set; }

        [Parameter]
        public Page Page { get; set; }

        protected override void OnInitialized()
        {
            bounditems = new List<BoundItem>();
            base.OnInitialized();
        }

        public void DisableSelection()
        {
            SelectionIsDisabled = true;
        }

        public void AddControlToBound(ControlData control)
        {
            foreach (var bond in bounditems)
            {
                if (control.Left > Left && control.Left < Right && control.Top > bond.Top && control.Top < bond.Bottom)
                {
                    control.BondType = bond.Data.BondType;
                    bond.Data.Controls.Add(control);
                    break;
                }
            }
        }

        public void AddTableToBound(TableData table)
        {
            //Last data boun or data header bound can be table
            foreach(var bond in bounditems.Where(t => t.DataLevel == Data.DataLevel || t.Data.BondType == BondType.DataHeader) )
            {
                if (table.Left > Left && table.Left < Right && table.Top > bond.Top && table.Top < bond.Bottom)
                {
                    table.BondType = bond.Data.BondType;
                    bond.Data.Table = table;
                    break;
                }
            }
        }

        void HideRulers()
        {
            verticalRulerLeft = null;
            verticalRulerRight = null;
            horizontalRulerTop = null;
            horizontalRulerBottom = null;
        }

        public double ColumnWidth 
        { 
            get 
            { 
                if (Data.ColumnCount < 2)
                    return Page.Data.Width;
                return (Page.Data.Width - (Data.ColumnCount - 1) * Data.ColumnGap) / Data.ColumnCount; 
            } 
        }

        public void ShowRuler(Table table, int x, out int left)
        {
            HideRulers();
            left = x;
            var list = new List<RecData>();
            foreach (var item in bounditems)
                item.GetRecDatas(list);
            foreach (var rect in list)
            {
                if (Math.Abs(rect.Left - x) < 6)
                {
                    verticalRulerLeft = left = (int)rect.Left;
                    break;
                }
                var right = rect.Left + rect.Width;
                if (Math.Abs(right - x) < 6)
                {
                    verticalRulerLeft = left = (int)right;
                    break;
                }
            }
        }

        public void ShowRuler(double? verticalRulerLeft)
        {
            this.verticalRulerLeft = verticalRulerLeft;
        }

        void ShowRuler(IList<RecData> recDatas, ControlData controlData, ref double width, ref double height, ChangeType change)
        {
            double left = controlData.Left, right = left + width, top = controlData.Top, bottom = top + height;
            foreach (var rect in recDatas)
            {
                var ctrRight = rect.Left + rect.Width;
                var ctrBottom = rect.Top + rect.Height;
                switch (change)
                {
                    case ChangeType.Move:
                        if (Math.Abs(left - rect.Left) < 6)
                        {
                            controlData.Left = rect.Left;
                            verticalRulerLeft = controlData.Left;
                        }
                        if (Math.Abs(right - ctrRight) < 6)
                        {
                            controlData.Left = ctrRight - width;
                            verticalRulerRight = ctrRight;
                        }
                        if (Math.Abs(top - rect.Top) < 6)
                        {
                            controlData.Top = rect.Top;
                            horizontalRulerTop = controlData.Top;
                        }
                        if (Math.Abs(bottom - ctrBottom) < 6)
                        {
                            controlData.Top = ctrBottom - height;
                            horizontalRulerBottom = ctrBottom;
                        }
                        break;
                    case ChangeType.LeftResize:
                        if (Math.Abs(left - rect.Left) < 6)
                        {
                            controlData.Left = rect.Left;
                            width = right - rect.Left;
                            verticalRulerLeft = controlData.Left;
                        }
                        break;
                    case ChangeType.RightResize:
                        if (Math.Abs(right - ctrRight) < 6)
                        {
                            width = ctrRight - left;
                            verticalRulerRight = ctrRight;
                        }
                        break;
                    case ChangeType.TopResize:
                        if (Math.Abs(top - rect.Top) < 6)
                        {
                            controlData.Top = rect.Top;
                            height = bottom - rect.Top;
                            horizontalRulerTop = controlData.Top;
                        }
                        break;
                    case ChangeType.BottomResize:
                        if (Math.Abs(bottom - ctrBottom) < 6)
                        {
                            height = ctrBottom - top;
                            horizontalRulerBottom = ctrBottom;
                        }
                        break;
                }
            }
        }

        public void ShowRuler(ControlData controlData, ref double width, ref double height, ChangeType change)
        {
            HideRulers();
            var list = new List<RecData>();
            foreach (var item in bounditems)
                item.GetRecDatas(list);
            ShowRuler(list, controlData, ref width, ref height, change);
        }

        public void Drag(double x, double y)
        {
            if (canChangeHeight && Page.IsMouseDown)
            {
                var difHeight = y - yStart;
                Page.SelectedBound.Data.Height = (int)(heightStart + difHeight);
                ///Drag controls and tables
                foreach (var item in bounditems.Where(t => t.Data.BondType > Page.SelectedBound.Data.BondType))
                    item.UpdateTopOnBoundDrag((int)difHeight);
                SelectionIsDisabled = true;
            }
        }

        public void DragStart(double x, double y)
        {
            if (canChangeHeight)
            {
                yStart = y;
                heightStart = Page.SelectedBound.Data.Height;
                /// Initial For drag
                foreach (var bound in bounditems)
                    bound.SetTopStartForControlsAndTable();
            }
        }

        public string GetCursor(double x, double y)
        {
            if (!Page.IsMouseDown)
            {
                if (Page.SelectedBound == null)
                    canChangeHeight = false;
                else
                    canChangeHeight = Math.Abs(y - Page.SelectedBound.Bottom - 2) < 5;
            }
            return canChangeHeight ? "row-resize" : "default";
        }

        public void Drop(double x, double y)
        {
            HideRulers();
        }

        public void EnableSelecting()
        {
            SelectionIsDisabled = false;
        }
    }
}
