﻿using Caspian.UI;
using Caspian.Common;
using System.Reflection;
using Microsoft.JSInterop;
using System.ComponentModel;
using Caspian.Engine.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Caspian.Common.Extension;

namespace Caspian.Engine.WorkflowEngine
{
    public partial class FormGeneratorComponent
    {
        byte columnsCount;
        IList<HtmlRow> rows;
        int selectedRowIndex = -1;
        int selectedColIndex = -1;
        int selectedInnerRowIndex = -1;
        int selectedInnerColIndex = -1;
        IList<int> selectedColsIndex;
        SubSystemKind subSystemKind;
        PropertySelector propertySelector;
        WindowStatus windowStatus;
        BlazorControl selectedControl;
        PropertyWindow propertyWindow;
        string formTitle;
        int dataModelId;
        // ------ Property Window
        string Id;
        string formName;
        IList<WorkflowForm> forms;

        void UpdateControlData()
        {

        }

        [Parameter]
        public EventCallback<string> OnEventHandlerCreated { get; set; }

        void SelectControl(BlazorControl ctr)
        {
            selectedControl = ctr;
            Id = ctr.GetId(subSystemKind);
        }

        void ToggleWindowStatus()
        {
            if (windowStatus == WindowStatus.Open)
                windowStatus = WindowStatus.Close;
            else
                windowStatus = WindowStatus.Open;
        }

        void DeleteSelectedRow()
        {
            if (selectedRowIndex >= 0)
                rows.RemoveAt(selectedRowIndex);
        }

        protected override void OnWindowClick()
        {
            propertySelector.HideSelector();
            StateHasChanged();
            base.OnWindowClick();
        }

        protected override async Task OnInitializedAsync()
        {
            using var scope = CreateScope();
            var formService = scope.GetService<WorkflowFormService>();
            var form = await formService.GetAll().Include(t => t.WorkflowGroup).SingleAsync(t => t.Id == WorkflowFormId);
            forms = await formService.GetAll().Where(t => t.WorkflowGroupId == form.WorkflowGroupId).ToListAsync();
            columnsCount = form.ColumnCount;
            subSystemKind = form.WorkflowGroup.SubSystemKind;
            formName = form.Name;
            formTitle = form.Title;
            dataModelId = form.DataModelId;
            rows = await scope.GetService<HtmlRowService>().GetRows(WorkflowFormId);
            selectedColsIndex = new List<int>();
            await base.OnInitializedAsync();
        }

        void SelectInnerRow(int rowIndex, int colIndex, int innerRowIndex, int innerColIndex)
        {
            selectedRowIndex = rowIndex;
            selectedColIndex = colIndex;
            selectedInnerRowIndex = innerRowIndex;
            selectedInnerColIndex = innerColIndex;
            selectedControl = null;
        }

        async Task SaveView()
        {
            using var scope = CreateScope();
            var service = scope.GetService<WorkflowFormService>();
            var rowService = scope.GetService<HtmlRowService>();
            var colService = scope.GetService<HtmlColumnService>();
            var innerRowService = scope.GetService<InnerRowService>();
            var columns = new List<HtmlColumn>();
            var columnsId = new List<int>();
            var transaction = await service.Context.Database.BeginTransactionAsync();

            foreach(var row in rows)
            {
                var rowCols = row.Columns;
                if (row.Id == 0)
                {
                    await rowService.AddAsync(row);
                    await rowService.SaveChangesAsync();
                    foreach(var col in rowCols)
                        col.RowId = row.Id;
                }
                foreach (var col in rowCols)
                    columns.Add(col);
            }
            var innerRows = new List<InnerRow>();
            foreach(var col in columns)
            {
                var colInnerRows = col.InnerRows;
                if (col.Id == 0)
                    await colService.AddAsync(col);
                else
                    await colService.UpdateAsync(col);
                await colService.SaveChangesAsync();
                columnsId.Add(col.Id);
                foreach (var row in colInnerRows)
                {
                    row.HtmlColumnId = col.Id;
                    innerRows.Add(row);
                }
            }
            columns = new List<HtmlColumn>();
            foreach (var row in innerRows)
            {
                var rowCols = row.HtmlColumns;
                if (row.Id == 0)
                {
                    await innerRowService.AddAsync(row);
                    await innerRowService.SaveChangesAsync();
                }
                foreach (var col in rowCols)
                {
                    col.InnerRowId = row.Id;
                    columns.Add(col);
                }
            }
            foreach (var col in columns)
            {
                if (col.Id == 0)
                    await colService.AddAsync(col);
                else
                    await colService.UpdateAsync(col);
                await colService.SaveChangesAsync();
                columnsId.Add(col.Id);
            }
            var deleteColumns = await colService.GetAll().Where(t => (t.Row.WorkflowFormId == WorkflowFormId 
                || t.InnerRow.HtmlColumn.Row.WorkflowFormId == WorkflowFormId) && !columnsId.Contains(t.Id))
                .Include(t => t.Component).Include(t => t.InnerRow).ToListAsync();
            colService.RemoveRange(deleteColumns);
            innerRowService.RemoveRange(deleteColumns.Where(t => t.InnerRow != null).Select(t => t.InnerRow));
            await colService.SaveChangesAsync();
            await transaction.CommitAsync();
        }

        void AddColumn()
        {
            if (selectedInnerRowIndex >= 0)
            {
                var factor = 12 / columnsCount;
                var cell = rows[selectedRowIndex].Columns[selectedColIndex];
                var colCount = cell.Span / factor;
                if (colCount > 1)
                {
                    var row = cell.InnerRows[selectedInnerRowIndex];
                    row.HtmlColumns[0].Span = Convert.ToByte(12 / colCount);
                    while (row.HtmlColumns.Count < colCount)
                    {
                        row.HtmlColumns.Add(new HtmlColumn()
                        {
                            Span = Convert.ToByte(12 / colCount)
                        });
                    }
                }
            }
        }

        void AddRowToTop()
        {
            var index = selectedRowIndex >= 0 ? selectedRowIndex : rows.Count > 0 ? 0 : (int?)null;
            AddRow(index);
        }

        void RemoveSelectedControl()
        {
            if (selectedControl != null)
            {
                foreach(var row in rows)
                {
                    foreach(var col in row.Columns)
                    {
                        if (col.Component == selectedControl)
                        {
                            col.Component = null;
                            selectedControl = null;
                            return;
                        }
                        foreach(var innerRow in col.InnerRows)
                        {
                            foreach (var innerCol in innerRow.HtmlColumns)
                                if (innerCol.Component == selectedControl)
                                {
                                    innerCol.Component = null;
                                    selectedControl = null;
                                    return;
                                }
                        }
                    }
                }
            }
        }

        void AddRowToDown()
        {
            var index = selectedRowIndex >= 0 && rows.Count > 0 ? selectedRowIndex + 1 : (int?)null;
            AddRow(index);
        }

        void AddRow(int? index = null)
        {
            var row = new HtmlRow()
            {
                WorkflowFormId = WorkflowFormId,
                Columns = new List<HtmlColumn>()
            };
            for (var index1 = 0; index1 < columnsCount; index1++)
                row.Columns.Add(new HtmlColumn()
                {
                    Span = Convert.ToByte(12 / columnsCount),
                    InnerRows = new List<InnerRow>()
                });
            if (index == null)
                rows.Add(row);
            else
                rows.Insert(index.Value, row);
        }

        void AddInnerRowToTop()
        {
            if (selectedRowIndex >= 0 && selectedColIndex >= 0)
            {
                var index = selectedInnerRowIndex >= 0 ? selectedInnerRowIndex : (int?)null;
                AddInnerRow(index);
            }
        }

        void AddInnerRowToDown()
        {
            var index = selectedInnerRowIndex >= 0 ? selectedInnerRowIndex + 1 : (int?)null;
            AddInnerRow(index);
        }

        void AddInnerRow(int? index = null)
        {
            var innerRow = new InnerRow();
            innerRow.HtmlColumns = new List<HtmlColumn>();
            innerRow.Span = 1;
            var innerCell = new HtmlColumn();
            innerCell.Span = 12;
            innerRow.HtmlColumns.Add(innerCell);
            if (rows[selectedRowIndex].Columns[selectedColIndex].Component != null)
                innerCell.Component = rows[selectedRowIndex].Columns[selectedColIndex].Component;
            rows[selectedRowIndex].Columns[selectedColIndex].Component = null;
            if (index == null)
                rows[selectedRowIndex].Columns[selectedColIndex].InnerRows.Add(innerRow);
            else
                rows[selectedRowIndex].Columns[selectedColIndex].InnerRows.Insert(index.Value, innerRow);
        }

        void RemoveInnerRow()
        {
            if (selectedRowIndex >= 0 && selectedColIndex >= 0 && selectedInnerRowIndex >= 0)
                rows[selectedRowIndex].Columns[selectedColIndex].InnerRows.RemoveAt(selectedInnerRowIndex);
        }

        void AddControl(DataModelField field)
        {
            ControlType? controlType = null; 
            switch(field.FieldType)
            {
                case DataModelFieldType.String:
                    controlType = ControlType.String;
                    break;
                case DataModelFieldType.Date:
                    controlType = ControlType.Date;
                    break;
                case DataModelFieldType.Time:
                    controlType = ControlType.Time;
                    break;
                case DataModelFieldType.Decimal:
                    controlType = ControlType.Numeric;
                    break;
                case DataModelFieldType.Integer:
                    controlType = ControlType.Integer;
                    break;
                case DataModelFieldType.Relational:
                    controlType = ControlType.List;
                    break;
                case DataModelFieldType.MultiOptions:
                    controlType = ControlType.DropdownList;
                    break;
                default:
                    throw new NotImplementedException("خطای عدم پیاده سازی");
            }
            var component = new Caspian.Engine.BlazorControl()
            {
                Caption = field.Title,
                ControlType = controlType.Value,
                DataModelFieldId = field.Id,
                CustomeFieldName = field.FieldName
            };
            if (selectedInnerRowIndex >= 0)
            {
                if (selectedColIndex >= 0 && selectedInnerColIndex >= 0)
                    rows[selectedRowIndex].Columns[selectedColIndex].InnerRows[selectedInnerRowIndex].HtmlColumns[selectedInnerColIndex].Component = component;
            }
            else if (selectedRowIndex >= 0 && selectedColIndex >= 0)
                rows[selectedRowIndex].Columns[selectedColIndex].Component = component;
        }

        async Task AddControl(PropertyInfo info)
        {
            var att = info.GetCustomAttribute<DisplayNameAttribute>();
            var component = new Caspian.Engine.BlazorControl()
            {
                ControlType = info.GetControlType(),
                Caption = att == null ? info.Name : att.DisplayName,
                PropertyName = info.Name,
                DataModelFieldId = propertySelector.GetSelectedDataModelFieldId().Value
            };
            using var service = CreateScope().GetService<DataModelFieldService>();
            component.DataModelField = await service.SingleAsync(component.DataModelFieldId);
            if (selectedInnerRowIndex >= 0)
            {
                if (selectedColIndex >= 0 && selectedInnerColIndex >= 0)
                    rows[selectedRowIndex].Columns[selectedColIndex].InnerRows[selectedInnerRowIndex].HtmlColumns[selectedInnerColIndex].Component = component;
            }
            else if (selectedRowIndex >= 0 && selectedColIndex >= 0)
                rows[selectedRowIndex].Columns[selectedColIndex].Component = component;
        }

        void SelectRow(MouseEventArgs e, int rowIndex, int colIndex)
        {
            selectedInnerRowIndex = -1;
            selectedControl = null;
            if (e.CtrlKey)
            {
                ///include first time
                if (selectedColIndex > -1)
                {
                    if (selectedRowIndex == rowIndex)
                    {
                        ///
                        if (colIndex != selectedColIndex)
                        {
                            selectedColsIndex.Add(colIndex);
                            selectedColsIndex.Add(selectedColIndex);
                        }
                    }
                    else
                        selectedColsIndex.Add(colIndex);
                }
                else
                {
                    if (rowIndex != selectedRowIndex)
                    {
                        selectedColsIndex.Clear();
                        selectedColsIndex.Add(colIndex);
                    }
                    else
                    {
                        if (selectedColsIndex.Contains(colIndex))
                            selectedColsIndex.Remove(colIndex);
                        else
                            selectedColsIndex.Add(colIndex);
                    }
                }
                selectedColIndex = -1;
            }
            else
            {
                selectedColsIndex.Clear();
                selectedColIndex = colIndex;
            }
            selectedRowIndex = rowIndex;
        }

        public void UnmergeSelectedColumns()
        {
            var columnIndex = selectedColIndex;
            if (columnIndex < 0 && selectedColsIndex.Count > 0)
                columnIndex = selectedColsIndex[0];
            if (selectedRowIndex >= 0 && columnIndex >= 0)
            {
                var factor = 12 / columnsCount;
                if (selectedInnerRowIndex < 0)
                {
                    var cell = rows[selectedRowIndex].Columns[columnIndex];
                    var count = cell.Span / factor;
                    cell.Span = Convert.ToByte(factor);
                    for (var i = 1; i < count; i++)
                    {
                        var cell1 = rows[selectedRowIndex].Columns[columnIndex + i];
                        cell1.Hidden = false;
                        cell1.Span = Convert.ToByte(factor);
                    }
                }
                else
                {
                    var cell = rows[selectedRowIndex].Columns[selectedColIndex];
                    //cell.InnerRows[selectedInnerRowIndex].ColumnsCount = Convert.ToByte(cell.Span / factor);
                }
            }
        }

        public void MergeSelectedColumns()
        {
            if (selectedColsIndex.Count > 1)
            {
                var hasMerge = false;
                foreach (var index in selectedColsIndex)
                {
                    if (rows[selectedRowIndex].Columns[index].Span != 12 / columnsCount)
                    {
                        hasMerge = true;
                        break;
                    }
                }
                if (hasMerge)
                {
                    ShowMessage("لطفا ستونهای ادغام شده را به حالت پیش فرض برگردانده و سپس اقدام به ادغام نمائید");
                    return;
                }
                var orderedColsIndex = selectedColsIndex.OrderBy(t => t).ToArray();
                bool hasGap = false;
                for (var index = 1; index < selectedColsIndex.Count; index++)
                {
                    if (orderedColsIndex[index - 1] + 1 != orderedColsIndex[index])
                    {
                        hasGap = true;
                        break;
                    }
                }
                if (hasGap)
                    ShowMessage("خانه های جدول برای ادغام درست انتخاب نشده اند");
                else
                {
                    var firstColumnIndex = orderedColsIndex[0];
                    var length = 12 / columnsCount * orderedColsIndex.Length;
                    rows[selectedRowIndex].Columns[firstColumnIndex].Span = Convert.ToByte(length);
                    for (var index = 1; index < orderedColsIndex.Count(); index++)
                    {
                        var selectedColumnIndex = orderedColsIndex[index];
                        selectedColsIndex.Remove(selectedColumnIndex);
                        rows[selectedRowIndex].Columns[selectedColumnIndex].Hidden = true;
                    }
                }
            }
        }

        [Parameter]
        public int WorkflowFormId { get; set; }

        [Parameter]
        public string Source { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await jsRuntime.InvokeVoidAsync("$.workflowForm.init", DotNetObjectReference.Create(this));
            await base.OnAfterRenderAsync(firstRender);
        }

        public async Task SaveCodeAndView()
        {
            var path = Environment.ContentRootPath + "\\Data\\Code\\";
            using var service = CreateScope().GetService<WorkflowFormService>();
            var form = await service.SingleAsync(WorkflowFormId);
            var datas = new CodeManager().GetExpressionData(form.Name, Source);
            foreach (var data in datas)
            {
                var control = GetControl(data.Id);
                if (control != null)
                {
                    if (data.PropertyName == "TextExpression")
                        control.TextExpression = data.Expression;
                    else if (data.PropertyName == "ConditionExpression")
                        control.ConditionExpression = data.Expression;
                }
            }
            if (!form.SourceFileName.HasValue())
            {
                form.SourceFileName = Path.GetRandomFileName();
                await service.SaveChangesAsync();
            }
            File.WriteAllText(path + form.SourceFileName + ".cs", Source);
            await SaveView();
            ShowMessage("ثبت با موفقیت انجام شد");
            StateHasChanged();
        }

        BlazorControl GetControl(string id)
        {
            foreach(var row in rows)
            {
                foreach(var col in row.Columns)
                {
                    var ctr = col.Component;
                    if (ctr == null && col.InnerRows != null)
                    {
                        foreach(var innerRow in col.InnerRows)
                        {
                            foreach(var col1 in innerRow.HtmlColumns)
                            {
                                if (col1.Component != null)
                                {
                                    var name = col1.Component.GetId(subSystemKind);
                                    if (name == id)
                                        return col1.Component;
                                }
                            }
                        }
                    } 
                    else 
                    {
                        var name = ctr.GetId(subSystemKind);
                        if (name == id)
                            return ctr;
                    }
                }
            }
            return null;
        }
    }
}
