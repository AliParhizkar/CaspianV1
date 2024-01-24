using Caspian.UI;
using Caspian.Report.Data;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

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

        string cursor = "default";

        public PageData Data { get; private set; }

        public Bound Bound { get; private set; }

        public BoundItem SelectedBound { get; private set; }

        public ReportControl SelectedControl { get; private set; }

        public Table SelectedTable { get; private set; }

        public void StateChanged()
        {
            StateHasChanged();
        }

        public bool IsMouseDown { get; private set; }

        protected override async Task OnInitializedAsync()
        {
            Data = await Host.GetFromJsonAsync<PageData>($"/ReportGenerator/GetReportData?reportId={ReportId}");
            await base.OnInitializedAsync();
        }

        [Parameter]
        public int ReportId { get; set; } = 1;

        void OnKeyDown(KeyboardEventArgs e)
        {
            if (SelectedControl != null)
            {
                var ctrKey = e.CtrlKey;
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
        }

        public bool WindowIsOpened { get; set; }

        public void OpenTextWindow()
        {
            status = WindowStatus.Open;
            isTextWindow = true;
            windowTitle = "Textbox Window";
            StateHasChanged();
        }

        public void OpenColumnWindow()
        {
            if (SelectedControl == null && SelectedTable == null)
            {
                status = WindowStatus.Open;
                isTextWindow = false;
                windowTitle = "Column Window";
                StateHasChanged();
            }

        }

        void CloseWindow(WindowStatus status)
        {
            this.status = status;
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
            }
        }

        async Task MouseDown(MouseEventArgs e)
        {
            await toolsBar.CloseDropdown();
            IsMouseDown = true;
            Bound.DragStart(e.ClientX, e.ClientY);
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
            cursor = Bound.GetCursor(e.ClientX, e.ClientY);
            if (IsMouseDown)
                Bound.Drag(e.ClientX, e.ClientY);
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

    }
}
