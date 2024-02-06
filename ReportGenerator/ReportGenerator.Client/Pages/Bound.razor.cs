using System.Data;
using Caspian.Report.Data;
using Caspian.Common.Extension;
using ReportGenerator.Client.Data;
using Microsoft.AspNetCore.Components;

namespace Caspian.Report
{
    public partial class Bound: ComponentBase
    {
        public const int Left = 182;
        public const int Top = 53;
        public const int BoundBetweenSpace = 3;

        BoundItem BoundItem { get { return default; } set { BoundItems.Add(value); } }
        
        double? horizontalRulerTop, horizontalRulerBottom, verticalRulerLeft, verticalRulerRight;
        
        bool canChangeHeight;
        double yStart, heightStart;
        double minHeightofSelectedBound;

        public int Right
        {
            get
            {
                return Left + Page.Data.Width;
            }
        }

        
        public IList<BoundItem> BoundItems { get; private set; }

        public bool SelectionIsDisabled { get; private set; }

        [Parameter]
        public BoundData Data { get; set; }

        [Parameter]
        public Page Page { get; set; }

        protected override void OnInitialized()
        {
            BoundItems = new List<BoundItem>();
            base.OnInitialized();
        }

        public void DisableSelection()
        {
            SelectionIsDisabled = true;
        }

        public void AddControlToBound(ControlData control)
        {
            foreach (var bond in BoundItems)
            {
                if (control.Left > Left && control.Left < Right && control.Top > bond.Top && control.Top < bond.Bottom)
                {
                    control.BondType = bond.Data.BondType;
                    bond.Data.Controls.Add(control);
                    control.Id = Page.GetId();
                    Page.Stack.Push(control.Id);
                    break;
                }
            }
        }

        public void AddTableToBound(TableData table)
        {
            //Last data boun or data header bound can be table
            foreach(var bond in BoundItems.Where(t => t.DataLevel == Data.DataLevel || t.Data.BondType == BondType.DataHeader) )
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
            foreach (var item in BoundItems)
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
            foreach (var item in BoundItems)
                item.GetRecDatas(list);
            ShowRuler(list, controlData, ref width, ref height, change);
        }

        public void Drop(double x, double y)
        {
            HideRulers();
        }

        public void UpdateControl(StackData stackData)
        {
            if (stackData.BoundItem != null)
            {
                var bound = BoundItems.SingleOrDefault(t => t.Data.BondType == stackData.BoundItem.BondType);
                Page.SelectBound(bound);
                bound.Data.Height = stackData.BoundItem.Height;
            }
            else if (stackData.Bound != null) 
            {
            
            }
            else if (stackData.Control != null)
            {
                var control = GetControlById(stackData.Id);
                if (control == null)
                {
                    var bondType = stackData.Control.BondType;
                    var bound = Data.Items.Single(t => t.BondType == bondType);
                    bound.Controls.Add(stackData.Control);
                }
                else
                {
                    control.Data.FullCopy(stackData.Control);
                    Page.SelectControl(control);
                }
            }
            else
            {
                var control = GetControlById(stackData.Id);
                control.BoundItem.Data.Controls.Remove(control.Data);
                control.BoundItem.ReportControls.Remove(control);
            }
        }

        public ReportControl GetControlById(string Id)
        {
            foreach(var item in BoundItems)
            {
                var control = item.ReportControls.SingleOrDefault(t => t.Data.Id == Id);
                if (control != null)
                    return control;
            }
            return null;
        }

        public void EnableSelecting()
        {
            SelectionIsDisabled = false;
        }
    }
}
