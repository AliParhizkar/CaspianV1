using System.Data;
using System.Security.Cryptography;
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
        IDictionary<string, RecData> bondsData;

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

        public async Task AddControlToBound(ControlData control)
        {
            await UpdateBoundsData();
            foreach (var bondData in bondsData)
            {
                double left = bondData.Value.Left, right = bondData.Value.Right, top = bondData.Value.Top, 
                    bottom = bondData.Value.Bottom;
                if (control.Left > left && control.Left < right && control.Top > top && control.Top < bottom)
                {
                    control.BondType = GetBondType(bondData.Key);
                    bondControls.Add(control);
                    break;
                }
            }
        }

        public async Task AddTableToBound(int tableLeft, int tableTop, TableData data)
        {
            await UpdateBoundsData();
            var dataBindName = $"databond{DataLevel}";
            //Last data boun or data header bound can be table
            foreach(var bondData in bondsData.Where(t => t.Key == dataBindName || t.Key == "dataHeader") )
            {
                double left = bondData.Value.Left, right = bondData.Value.Right, top = bondData.Value.Top,
                    bottom = bondData.Value.Bottom;
                if (tableLeft > left && tableLeft < right && tableTop > top && tableTop < bottom)
                {
                    var bondType = GetBondType(bondData.Key);
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
                foreach(var control in bondControls)
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
                {
                    canChangeHeight = Math.Abs(y - selectedBondRecData.Bottom - 2) < 5;
                }
            }
            return canChangeHeight ? "row-resize" : "default";
        }

        public void ResetBond()
        {
            selectedBond = null;
        }

        public async Task<RecData> GetBondDataAsync(BondType bondType)
        {
            var id = GetBondId(bondType);
            if (bondsData == null)
                await UpdateBoundsData();
            return bondsData[id];
        }

        public RecData GetBondData(BondType bondType)
        {
            var id = GetBondId(bondType);
            return bondsData[id];
        }

        public async Task Drop(double x, double y)
        {
            await UpdateBoundsData();
            if (selectedBond.HasValue)
            {
                var id = GetBondId(selectedBond.Value);
                selectedBondRecData = bondsData[id];
            }
            
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

        async Task SelectBond(BondType? bondType, string id)
        {
            if (Page.IsMouseDown && !selectionIsDisabled)
            {
                selectedBond = bondType;
                Page.ResetControl();
                Page.ResetTable();
                selectedBondRecData = await JSRuntime.GetClientRecById(id);
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

        async Task UpdateBoundsData()
        {
            var ids = new List<string>();
            if (reportTitleHeight > 0) 
                ids.Add(GetBondId(BondType.ReportTitle));
            if (pageHeaderHeight > 0)
                ids.Add(GetBondId(BondType.PageHeader));
            if (dataHeaderHeight > 0)
                ids.Add(GetBondId(BondType.DataHeader));
            if (firstDLHeight > 0)
                ids.Add(GetBondId(BondType.FirstDataLevel));
            if (secondDLHeight > 0)
                ids.Add(GetBondId(BondType.SecondDataLevel));
            if (thirdDLHeight > 0)
                ids.Add(GetBondId(BondType.ThirdDataLevel));
            if (dataFooterHeight > 0)
                ids.Add(GetBondId(BondType.DataFooter));
            if (pageFooterHeight > 0)
                ids.Add(GetBondId(BondType.PageFooter));
            if (bondsData == null)
                bondsData = new Dictionary<string, RecData>();
            foreach (var id in ids)
            {
                var data = await JSRuntime.GetClientRecById(id);
                bondsData[id] = data;
            }
        }

        public async Task UpdateBondSetting(ReportSetting setting)
        {
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
            StateHasChanged();
        }

        BondType GetBondType(string id)
        {
            switch (id)
            {
                case "databond1": return BondType.FirstDataLevel;
                case "databond2":  return BondType.SecondDataLevel;
                case "databond3":  return BondType.ThirdDataLevel;
                default: 
                    id = id.Substring(0, 1).ToUpper() + id.Substring(1); 
                    return (BondType)typeof(BondType).GetField(id).GetValue(null);
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await UpdateBoundsData();
                await Task.Delay(100);
                StateHasChanged();
            }
            await base.OnAfterRenderAsync(firstRender);
        }
    }
}
