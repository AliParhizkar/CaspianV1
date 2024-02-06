using Caspian.Common;
using Caspian.Report.Data;
using Microsoft.AspNetCore.Components;

namespace Caspian.Report
{
    public partial class BoundItem
    {
        bool canChangeHeight;
        double minHeight;
        double yStart, heightStart, topStart;
        bool statePushed;

        public Table Table { get; private set; }

        /// <summary>
        /// It return the top of boundItem
        /// </summary>
        public int Top 
        { 
            get 
            {
                var top = Bound.Top;
                foreach (var bound in Bound.Data.Items)
                {
                    if (bound.BondType == Data.BondType)
                        break;
                    top += bound.Height + Bound.BoundBetweenSpace;
                }
                return top;
            } 
        }

        public int Bottom => Top + Data.Height;

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
            {
                Data.Controls.Remove(Bound.Page.SelectedControl.Data);
                ReportControls.Remove(Bound.Page.SelectedControl);
            }
            else
                Data.Table = null;
        }

        public string GetCursor(double x, double y)
        {
            Console.WriteLine(Bottom);
            if (!Bound.Page.IsMouseDown)
            {
                if (Bound.Page.SelectedBound == this)
                    canChangeHeight = Math.Abs(y - Bottom - 2) < 5;
                else
                    canChangeHeight = false;
            }
            return canChangeHeight ? "row-resize" : "default";
        }

        public void DragStart(double x, double y)
        {
            if (canChangeHeight)
            {
                minHeight = MinHeight();
                yStart = y;
                heightStart = Data.Height;
                statePushed = false;
                /// Initial For drag
                foreach (var bound in Bound.BoundItems)
                    bound.SetTopStartForControlsAndTable();
            }
        }

        public void Drag(double x, double y)
        {
            if (canChangeHeight && Bound.Page.IsMouseDown)
            {
                if (!statePushed)
                {
                    statePushed = true;
                    Bound.Page.PushBound();
                }
                var difHeight = y - yStart;
                var height = (int)(heightStart + difHeight);
                if (height >= minHeight && height >= 24)
                {
                    Data.Height = height;
                    ///Drag controls and tables
                    foreach (var item in Bound.BoundItems.Where(t => t.Data.BondType > Data.BondType))
                        item.UpdateTopOnBoundDrag((int)difHeight);
                }
                Bound.DisableSelection();
            }
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

        /// <summary>
        /// We Use this method for Ruler showing 
        /// It fill the input list base on Table Cell  and Controls location
        /// </summary>
        /// <param name="list"></param>
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

        /// <summary>
        /// This method is used for bound dragging 
        /// on bound drag start we set TopStart to use on dragging
        /// </summary>
        public void SetTopStartForControlsAndTable()
        {
            if (Table != null)
                Table.TopStart = Table.Data.Top;
            foreach(var control in ReportControls)
                control.TopStart = control.Data.Top;
        }

        /// <summary>
        /// this method return the min height of the bound base on its controls and table
        /// </summary>
        /// <returns></returns>
        public double MinHeight()
        {
            double max = 0;
            if (Table != null)
                max = 22 + Table.Data.Rows.Sum(t => t.Height);
            foreach(var control in Data.Controls)
            {
                if (control.Top + control.Height - Top > max)
                    max = control.Top + control.Height - Top;
            }
            return max;
        }

        /// <summary>
        /// After table and controls height increasing we need to incraese the bound height
        /// </summary>
        public void UpdateHeight()
        {
            var height = (int)MinHeight();
            if (height > Data.Height)
                Data.Height = height;
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

        /// <summary>
        /// This method is used befor adding bound to set Top Start
        /// Top Start used for updateing controls top after bound is changed(add-remove)
        /// </summary>
        public void SetTopStart()
        {
            topStart = Top;
        }

        /// <summary>
        /// After Bound Change(add|remove) we use this method to update Top of Controls and Table
        /// </summary>
        public void UpdateControlsTop()
        {
            if (Table != null)
                Table.Data.Top = Table.TopStart;
            var difTop = Top - topStart;
            foreach (var control in Data.Controls)
                control.Top += difTop; 
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
