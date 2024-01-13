using Microsoft.AspNetCore.Components;

namespace Caspian.Report
{
    public partial class Bound: ComponentBase
    {
        double reportTitleHeight = 80, pageHeaderHeight, dataHeaderHeight, firstDLHeight = 100, secondDLHeight, thirdDLHeight,
            dataFooterHeight, pageFooterHeight;
        bool selectionIsDisabled;
        BondType? selectedBond;
        
        double? horizontalRulerTop, horizontalRulerBottom, verticalRulerLeft, verticalRulerRight;
        
        IList<ReportControl> reportControls;
        RecData selectedBondRecData;
        bool canChangeHeight;
        double yStart, heightStart;
        IList<ControlData> bondControls;
        IDictionary<string, RecData> bondsData;

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

        void HideRulers()
        {
            verticalRulerLeft = null;
            verticalRulerRight = null;
            horizontalRulerTop = null;
            horizontalRulerBottom = null;
        }

        public void ShowRuler(ControlData controlData, ref double width, ref double height, ChangeType change)
        {
            HideRulers();
            
            double left = controlData.Left, right = left + width, top = controlData.Top, bottom = top + height;
            foreach(var control in reportControls)
            {
                if (Page.SelectedControl != control)
                {
                    var ctrRight = control.Left + control.Width;
                    var ctrBottom = control.Top + control.Height;
                    switch (change)
                    {
                        case ChangeType.Move:
                            if (Math.Abs(left - control.Left) < 6)
                            {
                                controlData.Left = control.Left;
                                verticalRulerLeft = controlData.Left;
                            }
                            if (Math.Abs(right - ctrRight) < 6)
                            {
                                controlData.Left = ctrRight - width;
                                verticalRulerRight = ctrRight;
                            }
                            if (Math.Abs(top - control.Top) < 6)
                            {
                                controlData.Top = control.Top;
                                horizontalRulerTop = controlData.Top;
                            }
                            if (Math.Abs(bottom - ctrBottom) < 6)
                            {
                                controlData.Top = ctrBottom - height;
                                horizontalRulerBottom = ctrBottom;
                            }
                            break;
                        case ChangeType.LeftResize:
                            if (Math.Abs(left - control.Left) < 6)
                            {
                                controlData.Left = control.Left;
                                width = right - control.Left;
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
                            if (Math.Abs(top - control.Top) < 6)
                            {
                                controlData.Top = control.Top;
                                height = bottom - control.Top;
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

        public void Drag(double x, double y)
        {
            if (canChangeHeight && Page.IsMouseDown)
            {
                var difHeight = y - yStart;
                UpdateSelectedBoundHeight(heightStart + difHeight);
                selectionIsDisabled = true;
            }
        }

        public void DragStart(double x, double y)
        {
            if (canChangeHeight)
            {
                yStart = y;
                heightStart = GetSelectedBoundHeight().Value;
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
            if (selectedBond.HasValue)
            {
                var id = GetBondId(selectedBond.Value);
                selectedBondRecData = await JSRuntime.GetClientRecById(id);
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
                Page.Text = selectedBondRecData == null;
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
            bondsData = new Dictionary<string, RecData>();
            foreach (var id in ids)
            {
                var data = await JSRuntime.GetClientRecById(id);
                bondsData.Add(id, data);
            }
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
    }
}
