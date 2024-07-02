using Caspian.UI;
using Caspian.Report.Data;
using System.Net.Http.Json;
using ReportGenerator.Client;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Caspian.Common;
using Microsoft.JSInterop;

namespace Caspian.Report
{
    public partial class Page: ComponentBase
    {
        WindowStatus status;
        ToolsBar toolsBar;
        ElementReference element;
        bool controlAdding, tableAdding;
        ReportControl creatingControl;
        Table creatingTable;
        TableData tableData;
        ToolsBox toolsBox;
        bool isTextWindow;
        string windowTitle;
        ControlData controlData;
        MessageBox messageBox;
        int windowWidth;
        double pixelsPerCentimetre;
        int controlId;
        string message;
        string cursor = "default";

        public PageData Data { get; private set; }

        public readonly ReportStack Stack = new ReportStack();

        public Bound Bound { get; private set; }

        public BoundItem SelectedBound { get; private set; }

        public ReportControl SelectedControl { get; private set; }

        public Table SelectedTable { get; private set; }

        public void StateChanged()
        {
            StateHasChanged();
        }

        public bool IsMouseDown { get; private set; }

        async Task FetchData()
        {
            try
            {
                Data = await Host.GetFromJsonAsync<PageData>($"/ReportGenerator/GetReportData?reportId={ReportId}");
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
            catch(Exception ex)
            {

            }

        }

        protected override async Task OnInitializedAsync()
        {
            await FetchData();
            await base.OnInitializedAsync();
        }

        [Parameter]
        public int ReportId { get; set; } = 1;

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
                    
                }
                if (e.Code == "KeyD")
                {
                    
                }
            }

            if (e.Code == "Delete")
                RemoveSelectedItem();

        }

        public bool WindowIsOpened { get; set; }

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
                windowTitle = "Textbox Window";
                windowWidth = 400;
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
                StateChanged();
            }
        }

        public async Task AddControl(ControlData control)
        {
            ResetAll();
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

        async Task MouseDown(MouseEventArgs e)
        {
            await toolsBar.CloseDropdown();
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

        public void SelectControl(ReportControl control)
        {
            ResetAll();
            SelectedControl = control;
            StateHasChanged();
        }

        public string GetId()
        {
            controlId++;
            return $"ctr{controlId}";
        }

        public void SelectTable(Table table)
        {
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
                pixelsPerCentimetre = await JSRuntime.InvokeAsync<double>("getPixelsPerCentimetre", null);
            if (Data != null)
                Data.Width = Convert.ToInt32(Data.Setting.PageWidth * pixelsPerCentimetre);
            if (message.HasValue())
            {
                await JSRuntime.InvokeVoidAsync("$.caspian.showMessage", message);
                message = null;
            }
            await base.OnAfterRenderAsync(firstRender);
        }
    }
}
