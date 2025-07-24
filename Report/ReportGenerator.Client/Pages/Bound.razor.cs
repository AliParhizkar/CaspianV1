using System.Data;
using Caspian.Report.Data;
using Caspian.Common.Client;
using ReportGenerator.Client.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Runtime.CompilerServices;

namespace Caspian.Report
{
    public partial class Bound: ComponentBase
    {
        public const int Left = 182;
        public const int Top = 53;
        public const int BoundBetweenSpace = 3;

        string message;

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

        public void ArrangeBoundItems()
        {
            var bondItems = Data.Items.Where(t => t.BondType != BondType.DataHeader).OrderBy(t => t.BondType).ToList();
            var dataHeader = Data.Items.SingleOrDefault(t => t.BondType == BondType.DataHeader);
            if (dataHeader != null)
            {
                var maxDataLevel = Data.Items.Where(t => t.BondType < BondType.DataFooter).Max(t => t.BondType.ConvertToInt().Value) - 3;
                var index = maxDataLevel + Data.Items.Where(t => t.BondType < BondType.DataHeader).Count();
                bondItems.Insert(index, dataHeader);
                Data.Items = bondItems;
            }
        }

        
        public IList<BoundItem> BoundItems { get; private set; }

        public bool SelectionIsDisabled { get; private set; }

        [Parameter]
        public BoundData Data { get; set; }

        public BoundItem LastBoundItem
        {
            get
            {
                return BoundItems.Single(t => t.DataLevel == MaxDataLevel);
            }
        }

        public int MaxDataLevel
        {
            get
            {
                return BoundItems.Where(t => t.DataLevel.HasValue).Max(t => t.DataLevel.Value);
            }
        }

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
                if (control.Left >= Left && control.Left < Right && control.Top >= bond.Top && control.Top < bond.Bottom)
                {
                    control.BondType = bond.Data.BondType;
                    /// Convert Absolute Position to relative Position
                    control.Left = control.Left - Bound.Left;
                    control.Top = control.Top - bond.Top;
                    
                    bond.Data.Controls.Add(control);
                    control.Id = Page.GetId();
                    Page.Stack.Push(control.Id);
                    break;
                }
            }
        }

        public void AddTableToBound(TableData table)
        {
            //Last data bound or data header bound can be table
            foreach(var bond in BoundItems.Where(t => t.DataLevel == Data.DataLevel || t.Data.BondType == BondType.DataHeader) )
            {
                if (table.Left > Left && table.Left < Right && table.Top > bond.Top && table.Top < bond.Bottom)
                {
                    if (bond.Data.ColumnsCount > 1)
                        message = "امکان اضافه کردن جدول به باندهای چند ستونی وجود ندارد.";
                    else
                    {
                        table.BondType = bond.Data.BondType;
                        bond.Data.Table = table;
                    }
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

        public void ShowRuler(Table table, int x, out int left)
        {
            HideRulers();
            left = x;
            var list = new List<RecData>();
            foreach (var item in BoundItems)
                item.GetRecDatas(list, false);
            foreach (var rect in list)
            {
                if (Math.Abs(rect.Left - x) < 6)
                {
                    left = (int)rect.Left;
                    verticalRulerLeft = left + Bound.Left;
                    break;
                }
                var right = rect.Left + rect.Width;
                if (Math.Abs(right - x) < 6)
                {
                    left = (int)right;
                    verticalRulerLeft = left + Bound.Left;
                    break;
                }
            }
        }

        void ShowRuler(IList<RecData> recDatas, ReportControl reportControl, ref double width, ref double height, ChangeType change)
        {
            var controlData = reportControl.Data;
            double left = controlData.Left, right = left + width, top = controlData.Top, bottom = top + height;
            
            foreach (var rect in recDatas)
            {
                var ctrRight = rect.Left + rect.Width;
                var ctrBottom = rect.Top + rect.Height;
                switch (change)
                {
                    case ChangeType.Move:
                        var offsetLeft = reportControl.BoundItem == null ? 0 : Bound.Left;
                        var offsetTop = reportControl.BoundItem?.Top ?? 0;
                        if (Math.Abs(left - rect.Left) < 6)
                        {
                            controlData.Left = rect.Left;
                            verticalRulerLeft = controlData.Left + offsetLeft;
                        }
                        if (Math.Abs(right - ctrRight) < 6)
                        {
                            controlData.Left = ctrRight - width;
                            if (verticalRulerLeft - offsetLeft != controlData.Left)
                                verticalRulerLeft = null;
                            verticalRulerRight = ctrRight + offsetLeft;
                        }
                        if (Math.Abs(top - rect.Top) < 6)
                        {

                            controlData.Top = rect.Top;
                            horizontalRulerTop = controlData.Top + offsetTop;
                            
                        }
                        if (Math.Abs(bottom - ctrBottom) < 6)
                        {
                            controlData.Top = ctrBottom - height;
                            if (horizontalRulerTop != controlData.Top + offsetTop)
                                horizontalRulerTop = null;
                            horizontalRulerBottom = ctrBottom + offsetTop;
                        }
                        break;
                    case ChangeType.LeftResize:
                        if (Math.Abs(left - rect.Left) < 6)
                        {
                            controlData.Left = rect.Left;
                            width = right - rect.Left;
                            verticalRulerLeft = controlData.Left + Bound.Left;
                        }
                        break;
                    case ChangeType.RightResize:
                        if (Math.Abs(right - ctrRight) < 6)
                        {
                            width = ctrRight - left;
                            verticalRulerRight = ctrRight + Bound.Left;
                        }
                        break;
                    case ChangeType.TopResize:
                        if (Math.Abs(top - rect.Top) < 6)
                        {
                            controlData.Top = rect.Top;
                            height = bottom - rect.Top;
                            horizontalRulerTop = controlData.Top + reportControl.BoundItem.Top;
                        }
                        break;
                    case ChangeType.BottomResize:
                        if (Math.Abs(bottom - ctrBottom) < 6)
                        {
                            height = ctrBottom - top;
                            horizontalRulerBottom = ctrBottom + reportControl.BoundItem.Top;
                        }
                        break;
                }
            }
        }

        public void ShowRuler(ReportControl reportControl, ref double width, ref double height, ChangeType change)
        {
            HideRulers();
            var list = new List<RecData>();
            foreach (var item in BoundItems)
                item.GetRecDatas(list, reportControl.Data.BondType == null);
            ShowRuler(list, reportControl, ref width, ref height, change);
        }

        public void Drop(double x, double y)
        {
            HideRulers();
        }

        public void UpdateControl(StackData stackData)
        {
            if (stackData.Table != null)
            {
                var bound = BoundItems.SingleOrDefault(t => t.Data.BondType == stackData.Table.BondType);
                bound.Data.Table = stackData.Table;
            }
            else if (stackData.BoundItem != null)
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

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (message.HasValue())
            {
                await JSRuntime.InvokeVoidAsync("caspian.common.showMessage", message);
                message = null;
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        public void EnableSelecting()
        {
            SelectionIsDisabled = false;
        }
    }
}
