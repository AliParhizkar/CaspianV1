using Caspian.Common;
using System.Reflection;
using Microsoft.JSInterop;
using System.ComponentModel;
using Caspian.Engine.Service;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Caspian.UI;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Caspian.Engine.WorkflowEngine
{
    public partial class WorkflowFormGenerator
    {
        byte columnsCount;
        IList<HtmlRow> rows;
        int selectedRowIndex = -1;
        int selectedColIndex = -1;
        int selectedInnerRowIndex = -1;
        int selectedInnerColIndex = -1;
        IList<int> selectedColsIndex;
        bool controlSelected;
        SubSystemKind subSystemKind;
        PropertySeledtor propertySeledtor;
        WindowStatus windowStatus;
        bool saveFile;
        BlazorControl selectedControl;
        PropertyWindow propertyWindow;
        // ------ Property Window
        string Id;
        string title;
        string? description;
        string? textExpression;
        string? filterExpression;
        ControlType controlType;
        string formName;
        string? OnChangeEventHandler;

        void UpdateControlData()
        {
            if (controlSelected)
            {
                selectedControl.Description = propertyWindow.Descript;
                selectedControl.Caption = propertyWindow.Title;
                selectedControl.OnChange = propertyWindow.OnChangeEventHandler;
                OnChangeEventHandler = propertyWindow.OnChangeEventHandler;
            }
        }
        
        async Task SelectControl(BlazorControl ctr, int rowIndex, int colIndex, int? innerRowIndex = null, int? innerColIndex = null)
        {
            selectedRowIndex = rowIndex;
            selectedColIndex = colIndex;
            controlSelected = true;
            using var scope = CreateScope();
            Id = await new BlazorControlService(scope).GetId(subSystemKind, ctr);
            title = ctr.Caption;
            description = ctr.Description;
            textExpression = ctr.TextExpression;
            filterExpression = ctr.FilterExpression;
            controlType = ctr.ControlType;
            OnChangeEventHandler = ctr.OnChange;
            selectedControl = ctr;
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

        protected override async Task OnInitializedAsync()
        {
            using var scope = CreateScope();
            var form = await new WorkflowFormService(scope).SingleAsync(WorkflowFormId);
            columnsCount = form.ColumnCount;
            subSystemKind = form.SubSystemKind;
            formName = form.Name;
            rows = await new HtmlRowService(scope).GetRows(WorkflowFormId);
            selectedColsIndex = new List<int>();
            await base.OnInitializedAsync();
        }

        void SelectInnerRow(int rowIndex, int colIndex, int innerRowIndex, int innerColIndex)
        {
            selectedRowIndex = rowIndex;
            selectedColIndex = colIndex;
            selectedInnerRowIndex = innerRowIndex;
            selectedInnerColIndex = innerColIndex;
        }

        async Task Save()
        {
            using var scope = CreateScope();
            var service = new WorkflowFormService(scope);
            await service.Remove(WorkflowFormId);
            foreach (var row in rows)
            {
                row.WorkflowForm = null;
                foreach(var col in row.Columns)
                {
                    if (col.Component != null)
                        col.Component.WfFormEntityField = null;
                }
            }
            await new HtmlRowService(scope).AddRangeAsync(rows);
            await service.SaveChangesAsync();
        }

        void SaveAll()
        {
            saveFile = true;
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
            //var parentSpan = rows[selectedRowIndex].Columns[selectedColIndex].Span;
            //var factor = 12 / columnsCount;
            //var count = parentSpan / factor;
            //for (var index = 0; index < count; index++)
            //{
            //    var innerCell = new HtmlColumn();
            //    innerCell.Span = Convert.ToByte(12 / factor);
            //    innerRow.HtmlColumns.Add(innerCell);
            //}
            var innerCell = new HtmlColumn();
            innerCell.Span = 12;
            innerRow.HtmlColumns.Add(innerCell);
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

        void AddControl(PropertyInfo info)
        {
            var att = info.GetCustomAttribute<DisplayNameAttribute>();
            var component = new Caspian.Engine.BlazorControl()
            {
                ControlType = info.GetControlType(),
                Caption = att == null ? info.Name : att.DisplayName,
                PropertyName = info.Name,
                WfFormEntityFieldId = propertySeledtor.GetSelectedWfEntityFieldId()
            };
            if (selectedInnerRowIndex >= 0)
            {
                if (selectedInnerColIndex < 0)
                {

                }
                //rows[selectedRowIndex].Columns[selectedColIndex].InnerRows[selectedInnerRowIndex].HtmlColumn = control;
            }
            else if (selectedRowIndex >= 0 && selectedColIndex >= 0)
                rows[selectedRowIndex].Columns[selectedColIndex].Component = component;
        }

        void SelectRow(MouseEventArgs e, int rowIndex, int colIndex)
        {
            selectedInnerRowIndex = -1;
            controlSelected = false;
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

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await jsRuntime.InvokeVoidAsync("$.workflow.WorkflowForm", DotNetObjectReference.Create(this));
            if (saveFile)
            {
                saveFile = false;
                await jsRuntime.InvokeVoidAsync("$.workflow.sendSaveRequest");
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        [JSInvokable]
        public async Task<string> GetCodebehindString()
        {
            using var scop = CreateScope();
            return await new WorkflowFormService(scop).GetCodebehindAsync(WorkflowFormId);
        }

        [JSInvokable]
        public async Task<string> GetSourceCodeString()
        {
            using var scop = CreateScope();
            var basePath = Environment.ContentRootPath;
            return await new WorkflowFormService(scop).GetSourceCode(basePath, WorkflowFormId);
        }

        [JSInvokable]
        public async Task SaveFile(string code)
        {
            var path = Environment.ContentRootPath + "Data\\Code\\";
            using var scop = CreateScope();
            var service = new WorkflowFormService(scop);
            var form = await service.SingleAsync(WorkflowFormId);
            if (code.HasValue())
            {
                var method = new CodeManager().GetInitializeMethod(form.Name, code);
                foreach (var item in method.Body.Statements)
                {
                    switch (item.Kind())
                    {
                        case SyntaxKind.ExpressionStatement:
                            var expr = (item as ExpressionStatementSyntax).Expression;
                            if (expr.Kind() == SyntaxKind.SimpleAssignmentExpression)
                            {
                                var expr1 = expr as AssignmentExpressionSyntax;
                                BlazorControl control = null;
                                string property = null;
                                if (expr1.Left.Kind() == SyntaxKind.SimpleMemberAccessExpression)
                                {
                                    var letf = expr1.Left as MemberAccessExpressionSyntax;
                                    var id = (letf.Expression as IdentifierNameSyntax).Identifier.Text;
                                    control = await GetControl(id);
                                    property = letf.Name.Identifier.Text;
                                }
                                if (expr1.Right.Kind() == SyntaxKind.SimpleLambdaExpression)
                                {
                                    var span = expr1.Right.FullSpan;
                                    var expression = code.Substring(span.Start, span.Length);
                                    if (property == "TextExpression")
                                        control.TextExpression = expression;
                                    else if (property == "FilterExpression")
                                        control.FilterExpression = expression;
                                }
                            }
                            break;
                        default:
                            throw new InvalidOperationException("خطای عدم پیاده سازی");
                    }
                }
            }
            await Save();
            if (!form.SourceFileName.HasValue())
            {
                form.SourceFileName = Path.GetRandomFileName();
                await service.SaveChangesAsync();
            }
            File.WriteAllText(path + form.SourceFileName + ".cs", code);
            ShowMessage("ثبت با موفقیت انجام شد");
            StateHasChanged();
        }

        async Task<BlazorControl> GetControl(string id)
        {
            using var scope = CreateScope();
            var service = new BlazorControlService(scope);
            foreach(var row in rows)
            {
                foreach(var col in row.Columns)
                {
                    var ctr = col.Component;
                    if (ctr != null)
                    {
                        var name = await service.GetId(subSystemKind, ctr);
                        if (name == id)
                            return ctr;
                    }
                }
            }
            return null;
        }
    }
}
