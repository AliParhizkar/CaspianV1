using Caspian.UI.Client;
using Microsoft.JSInterop;
using Caspian.Report.Data;
using System.Net.Http.Json;
using Caspian.Common.Client;
using ReportGenerator.Client;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Caspian.Report
{
    public partial class Page: ComponentBase
    {
        WindowStatus status;
        ElementReference element;
        bool controlAdding, tableAdding, isTextWindow;
        ReportControl creatingControl, controlForCopy;
        Table creatingTable;
        TableData tableData;
        ToolsBox toolsBox;
        string windowTitle, message, cursor = "default";
        ControlData controlData;
        MessageBox messageBox;
        int windowWidth, controlId;
        double pixelsPerCentimetre;
        
        public ReportPageData Data { get; private set; }
        
        public ToolsBar ToolsBar { get; private set; }

        public readonly ReportStack Stack = new ReportStack();

        public Bound Bound { get; private set; }

        public BoundItem SelectedBound { get; private set; }

        public ReportControl SelectedControl { get; private set; }

        public Table SelectedTable { get; private set; }

        public bool WindowIsOpened { get; set; }

        public bool IsMouseDown { get; private set; }

        [Parameter]
        public int ReportId { get; set; } = 1;

        public void StateChanged()
        {
            StateHasChanged();
        }

        protected override void OnInitialized()
        {
            Host.BaseAddress = new Uri(Navigator.BaseUri);
            base.OnInitialized();
        }

        async Task FetchData()
        {
            Data = await Host.GetFromJsonAsync<ReportPageData>($"/ReportGenerator/GetReportData?reportId={ReportId}");
            /// Set table row for each table cells
            var tables = Data.Bound.Items.Where(t => t.Table != null).Select(t => t.Table).ToList();
            foreach (var table in tables)
                foreach (var row in table.Rows)
                    foreach (var cell in row.Cells)
                        cell.Row = row;
            var maxId = Data.Bound.Items.Max(t => t.Controls.Max(t => t.Id));
            if (maxId != null)
            {
                controlId = Convert.ToInt32(maxId.Replace("ctr", ""));
            }
            Data.Width = Convert.ToInt32(Data.Setting.PageWidth * pixelsPerCentimetre);

        }

        protected override async Task OnInitializedAsync()
        {
            await FetchData();
            await base.OnInitializedAsync();
        }

        void OnKeyDown(KeyboardEventArgs e)
        {
            var ctrKey = e.CtrlKey;
            if (SelectedControl != null)
            {
                switch (e.Code)
                {
                    case "ArrowUp":
                        if (ctrKey)
                            SelectedControl.Resize(0, -1);
                        else
                            SelectedControl.Move(0, -1);
                        break;
                    case "ArrowDown":
                        if (ctrKey)
                            SelectedControl.Resize(0, 1);
                        else
                            SelectedControl.Move(0, 1);
                        break;
                    case "ArrowRight":
                        if (ctrKey)
                            SelectedControl.Resize(1, 0);
                        else
                            SelectedControl.Move(1, 0);
                        break;
                    case "ArrowLeft":
                        if (ctrKey)
                            SelectedControl.Resize(-1, 0);
                        else
                            SelectedControl.Move(-1, 0);
                        break;
                }
            }
            if (e.CtrlKey)
            {
                if (e.Code == "KeyC")
                {
                    controlForCopy = SelectedControl;
                }
                if (e.Code == "KeyV")
                {
                    if (controlForCopy != null)
                    {
                        var newControl = controlForCopy.Data.Copy();
                        newControl.Id = GetId();
                        newControl.Left += 5;
                        newControl.Top += 5;
                        controlForCopy.BoundItem.Data.Controls.Add(newControl);
                        Stack.Push(newControl.Id);
                    }
                }
                if (e.Code == "KeyZ")
                {
                    if (Stack.CanUndo)
                    {
                        var result = Stack.Undo();
                        Bound.UpdateControl(result);
                    }
                }
                if (e.Code == "KeyY")
                {
                }
            }

            if (e.Code == "Delete")
                RemoveSelectedItem();

        }

        public void OpenTextWindow()
        {
            status = WindowStatus.Open;
            isTextWindow = true;
            if (SelectedControl?.Data.ControlType == ControlType.PictureBox)
            {
                windowTitle = "Picturebox Window";
                windowWidth = 335;
            }
            else
            {
                if (SelectedTable?.BoundItem?.DataLevel > 0)
                    windowTitle = "Data Field Window";
                else
                    windowTitle = "Textbox Window";
                windowWidth = 500;
            }
            StateHasChanged();
        }

        public void OpenColumnWindow()
        {
            if (SelectedControl == null && SelectedTable == null)
            {
                var bondType = SelectedBound.Data.BondType;
                if (bondType != BondType.DataHeader && bondType != BondType.DataFooter)
                {
                    status = WindowStatus.Open;
                    isTextWindow = false;
                    windowTitle = "Column Window";
                    StateHasChanged();
                    windowWidth = 400;
                }
            }
        }

        public void PushControl()
        {
            if (SelectedControl != null)
                Stack.Push(SelectedControl);
            if (SelectedTable != null)
                Stack.Push(SelectedTable.Data);
        }

        public void PushBound()
        {
            Stack.Push(SelectedBound);
        }

        public void Undo()
        {
            var result = Stack.Undo();
            Bound.UpdateControl(result);
        }

        void CloseWindow(WindowStatus status)
        {
            this.status = status;
        }

        public void RemoveSelectedItem()
        {
            if (SelectedControl != null || SelectedTable != null)
            {
                if (SelectedControl != null)
                    PushControl();
                (SelectedControl?.BoundItem ?? SelectedTable?.BoundItem).RemoveSelectedItem();
                ResetAll();
                cursor = "default";
                StateChanged();
            }
        }

        public async Task AddControl(ControlData control)
        {
            ResetAll();
            control.Font = new Font("12", control.Font.Family = ToolsBar.GetDefaultFont());
            controlData = control;
            controlAdding = true;
            await Task.Delay(100);
            creatingControl.InitializeBeforAddedToPage();
        }

        public async Task AddTable(TableData table)
        {
            ResetAll();
            tableData = table;
            tableAdding = true;
            await Task.Delay(100);
        }

        public async Task Save()
        {
            if (await messageBox.Confirm("Do you want save the report?"))
            {
                Data.PixelsPerCentimetre = pixelsPerCentimetre;
                foreach (var item in Data.Bound.Items)
                    if (item.ColumnsCount == 0)
                        item.ColumnsCount = 1;
                await Host.PostAsJsonAsync($"/ReportGenerator/SaveReport", Data);
                message = "ثبت با موفقیت انجام شد.";
                StateChanged();
            }
        }

        void MouseClick(MouseEventArgs e)
        {
            if (controlAdding)
            {
                Bound.AddControlToBound(controlData);
                controlAdding = false;
            }
            if (tableAdding)
            {
                Bound.AddTableToBound(tableData);
                tableAdding = false;
            }
            Bound.EnableSelecting();
        }

        void MouseUp(MouseEventArgs e)
        {
            if (IsMouseDown)
            {
                Bound.Drop(e.ClientX, e.ClientY);
                IsMouseDown = false;
                (SelectedControl?.BoundItem ?? SelectedTable?.BoundItem)?.UpdateHeight();
            }
        }

        void MouseDown(MouseEventArgs e)
        {
            ToolsBar.CloseDropdown();
            IsMouseDown = true;
            SelectedBound?.DragStart(e.ClientX, e.ClientY);
            SelectedControl?.DragStart(e.ClientX, e.ClientY);
            SelectedTable?.DragStart(e.ClientX, e.ClientY);
        }

        public void MouseMove(MouseEventArgs e)
        {
            if (WindowIsOpened)
                return;
            if (controlAdding)
            {
                controlData.Left = e.ClientX;
                controlData.Top = e.OffsetY;
                creatingControl?.Drag(e.ClientX, e.ClientY);
            }
            if (tableAdding)
            {
                tableData.Left = (int)e.ClientX;
                tableData.Top = (int)e.ClientY;
            }
            if (SelectedBound != null)
            {
                cursor = SelectedBound.GetCursor(e.ClientX, e.ClientY);
                if (IsMouseDown)
                    SelectedBound.Drag(e.ClientX, e.ClientY);
            }
            if (SelectedControl != null)
            {
                cursor = SelectedControl.GetCursor(e.ClientX, e.ClientY);
                if (IsMouseDown)
                    SelectedControl.Drag(e.ClientX, e.ClientY);
            }
            if (SelectedTable != null)
            {
                cursor = SelectedTable.GetCursor(e.ClientX, e.ClientY);
                if (IsMouseDown)
                    SelectedTable.Drag(e.ClientX, e.ClientY);
            }
        }

        public void SelectControl(ReportControl control, bool changeColorPickers = false)
        {
            if (changeColorPickers)
            {
                var data = control.Data;
                ToolsBar.ChangeColor(data.Font.Color, data.Border.Color, data.BackgroundColor);
            }
            ResetAll();
            SelectedControl = control;
            StateHasChanged();
        }

        public string GetId()
        {
            controlId++;
            return $"ctr{controlId}";
        }

        public void SelectTable(Table table, bool changeColorPickers = false)
        {
            if (changeColorPickers)
                ToolsBar.ChangeColor(table.Font.Color, table.Border.Color, table.BackgroundColor);
            ResetAll();
            SelectedTable = table;
            StateHasChanged();
        }

        public void SelectBound(BoundItem boundItem)
        { 
            if (!Bound.SelectionIsDisabled)
            {
                ResetAll();
                SelectedBound = boundItem;
            }
        }

        public void ResetAll()
        {
            SelectedControl = null;
            SelectedTable = null;
            SelectedBound = null;
            StateHasChanged();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
                pixelsPerCentimetre = await JSRuntime.InvokeAsync<double>("caspian.common.getPixelsPerCentimetre", null);
            if (Data != null)
                Data.Width = Convert.ToInt32(Data.Setting.PageWidth * pixelsPerCentimetre);
            if (message.HasValue())
            {
                await JSRuntime.InvokeVoidAsync("caspian.common.showMessage", message);
                message = null;
            }
            await base.OnAfterRenderAsync(firstRender);
        }
    }
}
