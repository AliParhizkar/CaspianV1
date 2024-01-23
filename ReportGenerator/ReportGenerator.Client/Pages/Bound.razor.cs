using System.Data;
using Caspian.Common;
using Caspian.Report.Data;
using Microsoft.JSInterop;
using Caspian.Common.Extension;
using Microsoft.AspNetCore.Components;

namespace Caspian.Report
{
    public partial class Bound: Caspian.UI.BasePage
    {
        public const int Left = 182;
        public const int Top = 53;
        public const int BoundBetweenSpace = 3;

        BoundItem reportTitle, pageHeader, dataHeader, firstDataLevel, secondDataLevel, thirdDataLevel, dataFooter, pageFooter;
        bool selectionIsDisabled;
        BondType? selectedBond;
        ReportBound bond;
        double? horizontalRulerTop, horizontalRulerBottom, verticalRulerLeft, verticalRulerRight;
        
        IList<ReportControl> reportControls;
        RecData selectedBondRecData;
        bool canChangeHeight;
        double yStart, heightStart;
        IList<ControlData> bondControls;
        IList<RecData> bondsData;
        int columnCount = 0;
        int columnGap = 2;

        TableData dataHeaderBoundTable, dataBoundTable;
        Table HeaderBoundTable, BoundTable;

        public int Right
        {
            get
            {
                return Left + Width;
            }
        }

        [Parameter]
        public Page Page { get; set; }

        [Parameter]
        public int Width { get; set; }

        public BoundItem SelectedBond { get; private set; }

        public int GetTopBounnd(BondType bondType)
        {
            var top = Top;
            //if (reportTitle != null && bondType > BondType.ReportTitle)
            //    top += reportTitle.Height + BoundBetweenSpace;
            //if (pageHeader != null && bondType > BondType.PageHeader)
            //    top += pageHeader.Height + BoundBetweenSpace;
            //if (dataHeader != null && bondType > BondType.DataHeader)
            //    top += dataHeader.Height + BoundBetweenSpace;
            //if (firstDataLevel != null && bondType > BondType.FirstDataLevel)
            //    top += firstDataLevel.Height + BoundBetweenSpace;
            //if (secondDataLevel != null && bondType > BondType.SecondDataLevel)
            //    top += secondDataLevel.Height + BoundBetweenSpace;
            //if (thirdDataLevel != null && bondType > BondType.ThirdDataLevel)
            //    top += thirdDataLevel.Height + BoundBetweenSpace;
            //if (dataFooter != null && bondType > BondType.DataFooter)
            //    top += dataFooter.Height + BoundBetweenSpace;
            //if (pageFooter != null && bondType > BondType.PageFooter)
            //    top += pageFooter.Height + BoundBetweenSpace;
            return top;
        }

        protected override void OnInitialized()
        {
            bondControls = new List<ControlData>();
            reportControls = new List<ReportControl>();
            bond = Page.ReportPage.ReportBound;
            base.OnInitialized();
        }

        public ReportControl ReportControl
        {
            get { return default; }
            set 
            { 
                reportControls.Add(value);
            }
        }

        public void DisableSelection()
        {
            selectionIsDisabled = true;
        }

        public void AddControlToBound(ControlData control)
        {
            foreach (var bondData in bondsData)
            {
                double left = bondData.Left, right = bondData.Right, top = bondData.Top, 
                    bottom = bondData.Bottom;
                if (control.Left > left && control.Left < right && control.Top > top && control.Top < bottom)
                {
                    control.BondType = bondData.BondType;
                    bondControls.Add(control);
                    break;
                }
            }
        }

        public void AddTableToBound(TableData data)
        {
            //Last data boun or data header bound can be table
            foreach(var bondData in bondsData.Where(t => t.BondType.ConvertToInt() == bond.DataLevel + 2 || t.BondType == BondType.DataHeader) )
            {
                double left = bondData.Left, right = bondData.Right, top = bondData.Top,
                    bottom = bondData.Bottom;
                if (data.Left > left && data.Left < right && data.Top > top && data.Top < bottom)
                {
                    var bondType = bondData.BondType;
                    data.BondType = bondType;
                    if (bondType == BondType.DataHeader)
                    {
                        if (dataHeaderBoundTable == null)
                            dataHeaderBoundTable = data;
                    }
                    else
                    {
                        if (dataBoundTable == null)
                            dataBoundTable = data;
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

        public int ColumnCount { get { return columnCount; } }

        public double ColumnWidth 
        { 
            get 
            {
                if (ColumnCount <= 1) 
                    return Width;
                return (Width - (columnCount - 1) * columnGap) / columnCount;
            } 
        }

        public void ShowRuler(Table table, int x, out int left)
        {
            Table otherTable = HeaderBoundTable == table ? BoundTable : HeaderBoundTable;
            left = x;
            verticalRulerLeft = null;
            if (otherTable != null)
            {
                var total = otherTable.Data.Left + 15;
                foreach (var cell in otherTable.Data.HeaderCells)
                {
                    if (Math.Abs(total - x) < 6)
                    {
                        verticalRulerLeft = left = total;
                        return;
                    }
                    total += cell.Width;
                }
            }
            foreach (var control in reportControls)
            {
                if (Math.Abs(control.Data.Left - x) < 6)
                {
                    verticalRulerLeft = left = (int)control.Data.Left;
                    break;
                }
                var right = control.Data.Left + control.Width;
                if (Math.Abs(right - x) < 6)
                {
                    verticalRulerLeft = left = (int)right;
                    break;
                }
            }
        }

        public ReportSetting GetReportSetting()
        {
            return new ReportSetting()
            {
                ReportTitle = bond.TitleHeight.HasValue,
                PageHeader = bond.PageHeaderHeight.HasValue,
                DataHeader = bond.DataHeaderHeight.HasValue,
                DataFooter = bond.DataFooterHeight.HasValue,
                PageFooter = bond.PageFooterHeight.HasValue,
                PageType = ReportPageType.B4
            };
        }

        void ShowRuler(IList<RecData> recDatas, ControlData controlData, ref double width, ref double height, ChangeType change)
        {
            double left = controlData.Left, right = left + width, top = controlData.Top, bottom = top + height;
            foreach (var rect in recDatas)
            {
                ReportControl control = null;
                if (rect.Bottom > 0)
                    control = reportControls.ElementAt((int)rect.Bottom - 1);
                if (Page.SelectedControl != control || control == null)
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
        }

        public void ShowRuler(ControlData controlData, ref double width, ref double height, ChangeType change)
        {
            HideRulers();
            var list = reportControls.Select((t, index) => new RecData()
            {
                Left = t.Data.Left,
                Top = t.Data.Top,
                Width = t.Width,
                Height = t.Height,
                Bottom = index + 1
            }).ToList();
            if (HeaderBoundTable != null)
                AddCellsRectToList(HeaderBoundTable, list);
            if (BoundTable != null)
                AddCellsRectToList(BoundTable, list);
            ShowRuler(list, controlData, ref width, ref height, change);
        }

        void AddCellsRectToList(Table table, IList<RecData> list)
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

        public void Drag(double x, double y)
        {
            if (canChangeHeight && Page.IsMouseDown)
            {
                var difHeight = y - yStart;
                UpdateSelectedBoundHeight(heightStart + difHeight);
                ///Drag controls and tables
                if (dataHeaderBoundTable != null && selectedBond.Value < BondType.DataHeader)
                    dataHeaderBoundTable.Top = HeaderBoundTable.TopStart + (int)(difHeight);
                if (dataBoundTable != null && selectedBond < BoundTable.Data.BondType)
                    dataBoundTable.Top = BoundTable.TopStart + (int)(difHeight);
                foreach (var control in bondControls)
                {
                    if (control.BondType.Value > selectedBond.Value)
                        control.Top = control.TopStart + (int)(difHeight);
                }

                selectionIsDisabled = true;
            }
        }

        public void DragStart(double x, double y)
        {
            if (canChangeHeight)
            {
                yStart = y;
                heightStart = GetSelectedBoundHeight().Value;
                /// Initial For drag
                if (HeaderBoundTable != null)
                    HeaderBoundTable.TopStart = HeaderBoundTable.Data.Top;
                if (dataBoundTable != null)
                    BoundTable.TopStart = dataBoundTable.Top;
                foreach(var control in bondControls)
                    control.TopStart = control.Top;
            }
        }

        public string GetCursor(double x, double y)
        {
            if (!Page.IsMouseDown)
            {
                if (selectedBond == null)
                    canChangeHeight = false;
                else
                    canChangeHeight = Math.Abs(y - selectedBondRecData.Bottom - 2) < 5;
            }
            return canChangeHeight ? "row-resize" : "default";
        }

        public void ResetBond()
        {
            selectedBond = null;
        }

        public RecData GetBondData(BondType bondType)
        {
            return bondsData.SingleOrDefault(t => t.BondType == bondType);
        }

        public void Drop(double x, double y)
        {
            if (selectedBond.HasValue)
                selectedBondRecData = GetBondData(selectedBond.Value);
            selectionIsDisabled = false;
            HideRulers();
        }

        double? GetSelectedBoundHeight()
        {
            switch (selectedBond)
            {
                case BondType.ReportTitle: return bond.TitleHeight;
                case BondType.PageHeader: return bond.PageHeaderHeight;
                case BondType.DataHeader: return bond.DataHeaderHeight;
                case BondType.FirstDataLevel: return bond.FirstDLHeight;
                case BondType.SecondDataLevel: return bond.SecondDLHeight;
                case BondType.ThirdDataLevel: return bond.ThirdDLHeight;
                case BondType.DataFooter: return bond.DataFooterHeight;
                case BondType.PageFooter: return bond.PageFooterHeight;
                default: return null;
            }
        }

        void SelectBond(BondType bondType)
        {
            if (Page.IsMouseDown && !selectionIsDisabled)
            {
                selectedBond = bondType;
                Page.ResetControl();
                Page.ResetTable();
                selectedBondRecData = GetBondData(bondType);
            }
        }

        void UpdateSelectedBoundHeight(double height)
        {
            switch (selectedBond)
            {
                case BondType.ReportTitle: bond.TitleHeight = height; break;
                case BondType.PageHeader: bond.PageHeaderHeight = height; break;
                case BondType.DataHeader: bond.DataHeaderHeight = height; break;
                case BondType.FirstDataLevel: bond.FirstDLHeight = height; break;
                case BondType.SecondDataLevel: bond.SecondDLHeight = height; break;
                case BondType.ThirdDataLevel: bond.ThirdDLHeight = height; break;
                case BondType.DataFooter: bond.DataFooterHeight = height; break;
                case BondType.PageFooter: bond.PageFooterHeight = height; break;
            }
        }

        public async Task UpdateBondSetting(ReportSetting setting)
        {
            var list = bondsData.Select(x => new
            {
                x.BondType,
                x.Top
            }).ToList();
            /// Adding Bounds to page
            if (setting.ReportTitle && bond.TitleHeight == null)
                bond.TitleHeight = 120;
            if (setting.PageHeader && bond.PageHeaderHeight == null)
                bond.PageHeaderHeight = 50;
            if (setting.DataHeader && bond.DataHeaderHeight == null)
                bond.DataHeaderHeight = 50;
            if (setting.DataFooter && bond.DataFooterHeight == null)
                bond.DataFooterHeight = 50;
            if (setting.PageFooter && bond.PageFooterHeight == null)
                bond.PageFooterHeight = 50;
            /// Remove Bounds from page
            if (!setting.ReportTitle && bond.TitleHeight.HasValue && await Confirm("1111"))
                bond.TitleHeight = 0;
            if (!setting.PageHeader && bond.PageHeaderHeight.HasValue && await Confirm("2222"))
                bond.PageHeaderHeight = 0;
            if (!setting.DataHeader && bond.DataHeaderHeight.HasValue && await Confirm("3333"))
                bond.DataHeaderHeight = 0;
            if (!setting.DataFooter && bond.DataFooterHeight.HasValue && await Confirm("444444"))
                bond.DataFooterHeight = 0;
            if (!setting.PageFooter && bond.PageFooterHeight.HasValue && await Confirm("555555"))
                bond.PageFooterHeight = 0;
            reportControls.Clear();
            await Task.Delay(1);
            var newList = bondsData.Select(x => new
            {
                BoundType = x.BondType,
                BoundTop = x.Top
            });
            
            bondControls = bondControls.Where(t => newList.Any(u => u.BoundType == t.BondType.Value)).ToList();
            foreach (var control in bondControls)
            {
                var bond = control.BondType.Value;
                var boundTop = list.Single(t => t.BondType == bond).Top;
                var newBoundTop = newList.Single(t => t.BoundType == bond).BoundTop;
                control.Top += newBoundTop - boundTop;
            }
            if (dataBoundTable != null)
            {
                var bondTop = list.Single(t => t.BondType.ConvertToInt() == bond.DataLevel + 2).Top;
                var boundNewType = newList.Single(t => t.BoundType.ConvertToInt() == bond.DataLevel + 2).BoundTop;
                dataBoundTable.Top += (int)(boundNewType - bondTop);
            }
            if (dataHeaderBoundTable != null)
            {
                var bondTop = list.Single(t => t.BondType == BondType.DataHeader).Top ;
                var boundNewType = newList.Single(t => t.BoundType == BondType.DataHeader).BoundTop;
                dataHeaderBoundTable.Top += (int)(boundNewType - bondTop);
            }
            StateHasChanged();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            bondsData = await JSRuntime.InvokeAsync<RecData[]>("getClientRecBounds");

            await base.OnAfterRenderAsync(firstRender);
        }
    }
}
