using System.Data;
using Caspian.Common;
using Microsoft.JSInterop;
using Caspian.Common.Extension;
using Microsoft.AspNetCore.Components;

namespace Caspian.Report
{
    public partial class Bound: Caspian.UI.BasePage
    {
        double reportTitleHeight, pageHeaderHeight, dataHeaderHeight, dataFooterHeight, pageFooterHeight;
        double firstDLHeight = 50, secondDLHeight, thirdDLHeight;
        bool selectionIsDisabled;
        BondType? selectedBond;
        
        double? horizontalRulerTop, horizontalRulerBottom, verticalRulerLeft, verticalRulerRight;
        
        IList<ReportControl> reportControls;
        RecData selectedBondRecData;
        bool canChangeHeight;
        double yStart, heightStart;
        IList<ControlData> bondControls;
        IList<RecData> bondsData;

        TableData dataHeaderBoundTable, dataBoundTable;
        Table HeaderBoundTable, BoundTable;

        [Parameter]
        public byte DataLevel { get; set; }

        [Parameter]
        public Page Page { get; set; }

        [Parameter]
        public double Width { get; set; }

        protected override void OnInitialized()
        {
            bondControls = new List<ControlData>();
            reportControls = new List<ReportControl>();

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

        public void AddTableToBound(int tableLeft, int tableTop, TableData data)
        {
            //Last data boun or data header bound can be table
            foreach(var bondData in bondsData.Where(t => t.BondType.ConvertToInt() == DataLevel + 2 || t.BondType == BondType.DataHeader) )
            {
                double left = bondData.Left, right = bondData.Right, top = bondData.Top,
                    bottom = bondData.Bottom;
                if (tableLeft > left && tableLeft < right && tableTop > top && tableTop < bottom)
                {
                    var bondType = bondData.BondType;
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
                var total = otherTable.Left + 15;
                foreach (var cell in otherTable.TableData.HeaderCells)
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
                if (Math.Abs(control.Left - x) < 6)
                {
                    verticalRulerLeft = left = (int)control.Left;
                    break;
                }
                var right = control.Left + control.Width;
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
                ReportTitle = reportTitleHeight > 0,
                PageHeader = pageHeaderHeight > 0,
                DataHeader = dataHeaderHeight > 0,
                DataFooter = dataFooterHeight > 0,
                PageFooter = pageFooterHeight > 0,
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
                Left = t.Left,
                Top = t.Top,
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
            var sumLeft = table.Left + 15;
            foreach (var cell in table.TableData.HeaderCells)
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
                    dataHeaderBoundTable.Top = dataHeaderBoundTable.TopStart + (int)(difHeight);
                if (dataBoundTable != null && selectedBond < BoundTable.BondType)
                    dataBoundTable.Top = dataBoundTable.TopStart + (int)(difHeight);
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
                if (dataHeaderBoundTable != null)
                    dataHeaderBoundTable.TopStart = dataHeaderBoundTable.Top;
                if (dataBoundTable != null)
                    dataBoundTable.TopStart = dataBoundTable.Top;
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
                case BondType.ReportTitle: return reportTitleHeight;
                case BondType.PageHeader: return pageHeaderHeight;
                case BondType.DataHeader: return dataHeaderHeight;
                case BondType.FirstDataLevel: return firstDLHeight;
                case BondType.SecondDataLevel: return secondDLHeight;
                case BondType.ThirdDataLevel: return thirdDLHeight;
                case BondType.DataFooter: return dataFooterHeight;
                case BondType.PageFooter: return pageFooterHeight;
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
                case BondType.ReportTitle: reportTitleHeight = height; break;
                case BondType.PageHeader: pageHeaderHeight = height; break;
                case BondType.DataHeader: dataHeaderHeight = height; break;
                case BondType.FirstDataLevel: firstDLHeight = height; break;
                case BondType.SecondDataLevel: secondDLHeight = height; break;
                case BondType.ThirdDataLevel: thirdDLHeight = height; break;
                case BondType.DataFooter: dataFooterHeight = height; break;
                case BondType.PageFooter: pageFooterHeight = height; break;
            }
        }

        string GetBondId(BondType bondType)
        {
            switch (bondType)
            {
                case BondType.FirstDataLevel: return "databond1";
                case BondType.SecondDataLevel: return "databond2";
                case BondType.ThirdDataLevel: return "databond3";
                default: string id = bondType.ToString(); return id.Substring(0, 1).ToLower() + id.Substring(1);
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
            if (setting.ReportTitle && reportTitleHeight == 0)
                reportTitleHeight = 120;
            if (setting.PageHeader && pageHeaderHeight == 0)
                pageHeaderHeight = 50;
            if (setting.DataHeader && dataHeaderHeight == 0)
                dataHeaderHeight = 50;
            if (setting.DataFooter && dataFooterHeight == 0)
                dataFooterHeight = 50;
            if (setting.PageFooter && pageFooterHeight == 0)
                pageFooterHeight = 50;
            /// Remove Bounds from page
            if (!setting.ReportTitle && reportTitleHeight > 0 && await Confirm("1111"))
                reportTitleHeight = 0;
            if (!setting.PageHeader && pageHeaderHeight > 0 && await Confirm("2222"))
                pageHeaderHeight = 0;
            if (!setting.DataHeader && dataHeaderHeight > 0 && await Confirm("3333"))
                dataHeaderHeight = 0;
            if (!setting.DataFooter && dataFooterHeight > 0 && await Confirm("444444"))
                dataFooterHeight = 0;
            if (!setting.PageFooter && pageFooterHeight > 0 && await Confirm("555555"))
                pageFooterHeight = 0;
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
                var bondTop = list.Single(t => t.BondType.ConvertToInt() == DataLevel + 2).Top;
                var boundNewType = newList.Single(t => t.BoundType.ConvertToInt() == DataLevel + 2).BoundTop;
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
