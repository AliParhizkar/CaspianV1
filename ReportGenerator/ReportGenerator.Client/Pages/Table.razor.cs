using Microsoft.JSInterop;
using Caspian.Report.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Caspian.Report
{
    public partial class Table: ComponentBase
    {
        IList<TableCellData> selectedCells;
        ChangeKind? changeKind;
        int? changeColIndex, leftCellStartWidth, rightCellStartWidth, xStart, leftStart, difrentCurcer;
        bool contextMenuStatus, statePushed;
        double left, top;
        string message;

        int? changeRowIndex, heightStart, yStart;

        [Parameter]
        public BoundItem BoundItem { get; set; }

        [Parameter]
        public Bound Bound { get; set; }

        public Alignment Alignment 
        { 
            get
            {
                var alignment = selectedCells.First().Alignment;
                return new Alignment()
                {
                    HorizontalAlign = selectedCells.All(t => t.Alignment.HorizontalAlign == alignment.HorizontalAlign) ? alignment.HorizontalAlign : 0,
                    VerticalAlign = selectedCells.All(t => t.Alignment.VerticalAlign == alignment.VerticalAlign) ? alignment.VerticalAlign : 0
                };
            }
            set
            {
                foreach(var cell in selectedCells)
                    Data.Rows[cell.Row.RowIndex - 1].Cells[cell.ColIndex - 1].Alignment = value;
            }
        }

        void ShowContextMenu(MouseEventArgs e)
        {
            contextMenuStatus = true;
            left = e.ClientX + 10;
            top = e.ClientY - 10;
        }

        public Font Font
        {
            get
            {
                var font = selectedCells.First().Font;
                var fontName = selectedCells.All(t => t.Font.Family == font.Family) ? font.Family : null;
                var fontSize = selectedCells.All(t => t.Font.Size == font.Size) ? font.Size : null;
                return new Font()
                {
                    Bold = selectedCells.All(t => t.Font.Bold),
                    Italic = selectedCells.All(t => t.Font.Italic),
                    UnderLine = selectedCells.All(t => t.Font.UnderLine),
                    Family = fontName,
                    Size = fontSize,
                    Color = selectedCells.All(t => t.Font.Color.ColorString == font.Color.ColorString) ? font.Color : new Color("#000")
                };
            }
            set
            {
                foreach (var cell in selectedCells)
                    Data.Rows[cell.Row.RowIndex - 1].Cells[cell.ColIndex - 1].Font = value;
            }
        }

        public Color BackgroundColor
        {
            get
            {
                var color = selectedCells.First().BackgroundColor;
                if (selectedCells.All(t => t.BackgroundColor.ColorString == color.ColorString))
                    return color;
                return new Color();
            }
            set
            {
                foreach (var cell in selectedCells)
                    Data.Rows[cell.Row.RowIndex - 1].Cells[cell.ColIndex - 1].BackgroundColor = value;
            }
        }

        public Border Border
        {
            get
            {
                var border = selectedCells.First().Border;
                BorderKind kind = 0;
                if (selectedCells.All(t => (t.Border.BorderKind & BorderKind.Top) == BorderKind.Top))
                    kind |= BorderKind.Top;
                if (selectedCells.All(t => (t.Border.BorderKind & BorderKind.Bottom) == BorderKind.Bottom))
                    kind |= BorderKind.Bottom;
                if (selectedCells.All(t => (t.Border.BorderKind & BorderKind.Left) == BorderKind.Left))
                    kind |= BorderKind.Left;
                if (selectedCells.All(t => (t.Border.BorderKind & BorderKind.Right) == BorderKind.Right))
                    kind |= BorderKind.Right;
                return new Border()
                {
                    BorderKind = kind,
                    BorderStyle = selectedCells.All(t => t.Border.BorderStyle == border.BorderStyle) ? border.BorderStyle : 0,
                    Width = selectedCells.All(t => t.Border.Width == border.Width) ? border.Width : 0,
                    Color = selectedCells.All(t => t.Border.Color.ColorString == border.Color.ColorString) ? border.Color : new Color("#000"),
                };
            }
            set
            {
                foreach (var cell in selectedCells)
                    Data.Rows[cell.Row.RowIndex - 1].Cells[cell.ColIndex - 1].Border = value;
            }
        }

        public TextFieldData FieldData
        {
            get
            {
                if (selectedCells.Count != 1)
                    return null;
                return selectedCells.Single().FieldData;
            }
            set
            {
                foreach (var cell in selectedCells)
                    cell.FieldData = value;
            }
        }

        public string Text
        {
            get
            {
                if (selectedCells.Count != 1)
                    return null;
                return selectedCells.Single().Text;
            }
            set
            {
                var cell = selectedCells.Single();
                Data.Rows[cell.Row.RowIndex - 1].Cells[cell.ColIndex - 1].Text = value;
            }
        }

        [Parameter]
        public TableData Data { get; set; }

        public int TopStart { get; set; }

        #region Add or Remove Column and row

        void InsertRow(bool insertAbove)
        {
            if (!TableMeged())
            {
                var index = selectedCells.First().Row.RowIndex;
                /// Insert row above or after the selected row
                var row = new TableRowData();
                for (var i = 1; i <= Data.HeaderCells.Count; i++)
                    row.Cells.Add(new TableCellData(row));
                Data.Rows.Insert(insertAbove ? index - 1 : index, row);
                UpdateRowAndColumnIndex();
            }
            else
                message = "For row inserting we need to unmerge table";
        }

        void RemoveRow()
        {
            if (!TableMeged() && Data.Rows.Count > 1)
            {
                var index = selectedCells.First().Row.RowIndex - 1;
                Data.Rows.RemoveAt(index);
                UpdateRowAndColumnIndex();
            }
            else
            {
                if (Data.Rows.Count == 1)
                    message = "Table should have at least a row";
                else
                    message = "For row removeing we need to unmerge table";
            }
        }

        void InsertColumn(bool insertBefor)
        {
            if (!TableMeged())
            {
                /// Insert column befor or after the selectd column
                var index = selectedCells.First().ColIndex;
                var tempIndex = insertBefor ? index - 1 : index;
                var head = Data.HeaderCells.ElementAt(index - 1);
                Data.HeaderCells.Insert(tempIndex, new HeaderCellData() { Width = head.Width / 2 });
                head.Width = head.Width - head.Width / 2;
                foreach (var row in Data.Rows)
                    row.Cells.Insert(tempIndex, new TableCellData(row));
                UpdateRowAndColumnIndex();
            }
            else
                message = "For column insertig we need to unmerge table";
        }

        void RemoveColumn()
        {
            if (!TableMeged() && Data.HeaderCells.Count > 1)
            {
                var index = selectedCells.First().ColIndex;
                var headerCells = Data.HeaderCells;
                var curent = headerCells.ElementAt(index - 1);
                if (index == 1)
                {
                    var next = headerCells.ElementAt(index);
                    next.Width += curent.Width;
                }
                else
                {
                    var pre = headerCells.ElementAt(index - 2);
                    pre.Width += curent.Width;
                }
                foreach (var row in Data.Rows)
                    row.Cells.RemoveAt(index - 1);
                headerCells.Remove(curent);
                UpdateRowAndColumnIndex();
            }
            else
            {
                if (Data.HeaderCells.Count == 1)
                    message = "Table should have at least a column";
                else
                    message = "For column removing we need to unmerge table";
            }
        }

        void UpdateRowAndColumnIndex()
        {
            var rowIndex = 1;
            foreach (var row in Data.Rows)
            {
                row.RowIndex = rowIndex;
                var colIndex = 1;
                foreach (var cell in row.Cells)
                {
                    cell.ColIndex = colIndex;
                    colIndex++;
                }
                rowIndex++;
            }
        }
        #endregion

        #region Merge and unmerge
        public void UnmergeSelectedCell()
        {
            var selectedCell = selectedCells.Single();
            if (selectedCell.ColSpan > 1)
            {
                foreach (var cell in selectedCell.Row.Cells)
                {
                    if (cell.ColIndex > selectedCell.ColIndex && cell.ColIndex <= selectedCell.ColIndex + selectedCell.ColSpan)
                        cell.Hidden = false;

                }
                selectedCell.ColSpan = 1;
            }
            if (selectedCell.RowSpan > 1)
            {
                for (var i = 0; i < selectedCell.RowSpan - 1; i++)
                    Data.Rows[selectedCell.Row.RowIndex + i].Cells[selectedCell.ColIndex - 1].Hidden = false;
                selectedCell.RowSpan = 1;
            }
        }

        public bool CanUnmerge()
        {
            if (selectedCells.Count != 1)
                return false;
            var cell = selectedCells.First();
            return cell.ColSpan > 1 || cell.RowSpan > 1;
        }

        public void MergeSelectedCells()
        {
            bool sameRow, sameCol;
            var result = CanMerge(out sameRow, out sameCol);
            if (result)
            {
                Bound.Page.Stack.Push(Data);
                if (sameRow)
                {
                    var min = selectedCells.Min(t => t.ColIndex);
                    var row = Data.Rows.ElementAt(selectedCells.First().Row.RowIndex - 1);
                    var index = 1;
                    foreach (var cell in row.Cells)
                    {
                        if (index == min)
                            cell.ColSpan = selectedCells.Count;
                        else if (selectedCells.Any(t => t.ColIndex == index))
                            cell.Hidden = true;
                        index++;
                    }
                }
                if (sameCol)
                {
                    var min = selectedCells.Min(t => t.Row.RowIndex);
                    var col = selectedCells.First().ColIndex - 1;
                    var index = 1;
                    foreach (var row in Data.Rows)
                    {
                        if (index == min)
                            row.Cells.ElementAt(col).RowSpan = selectedCells.Count;
                        else if (selectedCells.Any(t => t.Row.RowIndex == index))
                            row.Cells.ElementAt(col).Hidden = true;
                        index++;
                    }
                }
            }
        }

        bool TableMeged()
        {
            foreach (var row in Data.Rows)
                if (row.Cells.Any(t => t.RowSpan > 1 || t.ColSpan > 1))
                    return true;
            return false;
        }

        public bool CanMerge(out bool sameRow, out bool sameCol)
        {
            sameRow = true;
            sameCol = true;
            if (selectedCells.Count < 2)
                return false;
            if (selectedCells.Max(t => t.RowSpan) > 1 || selectedCells.Max(t => t.ColSpan) > 1)
                return false;
            var firstCell = selectedCells.First();
            foreach (var cell in selectedCells)
            {
                if (cell.Row.RowIndex != firstCell.Row.RowIndex)
                {
                    sameRow = false;
                    break;
                }
            }
            foreach (var cell in selectedCells)
            {
                if (cell.ColIndex != firstCell.ColIndex)
                {
                    sameCol = false;
                    break;
                }
            }
            if (sameCol)
            {
                /// difrent max and min should be equal with selected cells count
                var difrent = selectedCells.Max(t => t.Row.RowIndex) - selectedCells.Min(t => t.Row.RowIndex) + 1;
                if (difrent == selectedCells.Count)
                    return true;
            }
            if (sameRow)
            {
                var difrent = selectedCells.Max(t => t.ColIndex) - selectedCells.Min(t => t.ColIndex) + 1;
                if (difrent == selectedCells.Count)
                    return true;
            }
            return false;
        }
        #endregion

        public NumberFormating GetNumberFormating()
        {
            if (selectedCells.Count == 1)
            {
                var cell = selectedCells.Single();
                return cell.NumberFormating;
            }
            
            return null;
        }

        public void SetFormating(NumberFormating formating)
        {
            foreach (var cell in selectedCells)
                cell.NumberFormating = formating;
        }

        protected override void OnInitialized()
        {
            selectedCells = new List<TableCellData>();
            base.OnInitialized();
        }

        void SelectTableCell(bool ctrKey, TableCellData cell)
        {
            if (ctrKey)
            {
                if (selectedCells.Contains(cell))
                    selectedCells.Remove(cell);
                else
                    selectedCells.Add(cell);
            }
            else
            {
                selectedCells.Clear();
                selectedCells.Add(cell);
            }
            Bound.DisableSelection();
            if (selectedCells.Count > 0)
                Bound.Page.SelectTable(this);
            else
                Bound.Page.ResetAll();
        }

        public string GetCursor(double x, double y)
        {
            if (Bound.Page.SelectedTable == this)
            {
                if (y > Data.Top && y < Data.Top + 20)
                {
                    var leftColl = Data.Left + 15;
                    if (Math.Abs(leftColl - x) < 6)
                        return "col-resize";
                    foreach (var cel in Data.HeaderCells)
                    {
                        leftColl += cel.Width;
                        if (Math.Abs(leftColl - x) < 6)
                            return "col-resize";
                    }
                }
                if (x > Data.Left && x < Data.Left + 15)
                {
                    var topRow = Data.Top + 20;
                    foreach (var row in Data.Rows)
                    {
                        topRow += row.Height;
                        if (Math.Abs(topRow - y) < 5)
                            return "row-resize";
                    }
                }
            }
            return "default";
        }

        public void DragStart(double x, double y)
        {
            if (Bound.Page.SelectedTable == this)
            {
                changeKind = null;
                ///Column resize
                if (y > Data.Top && y < Data.Top + 20)
                {
                    var leftCell = Data.Left + 15;
                    if (Math.Abs(leftCell - (int)x) < 6)
                    {
                        changeKind = ChangeKind.ColumnResize;
                        changeColIndex = 1;
                        difrentCurcer = leftCell - (int)x;
                    }
                    var index = 2;
                    foreach (var cel in Data.HeaderCells)
                    {
                        leftCell += cel.Width;
                        if (Math.Abs(leftCell - (int)x) < 6)
                        {
                            changeKind = ChangeKind.ColumnResize;
                            changeColIndex = index;
                            difrentCurcer = leftCell - (int)x;
                        }
                        index++;
                    }

                }
                ///row resize
                if (x > Data.Left && x < Data.Left + 15)
                {
                    var topRow = Data.Top + 20;
                    var index = 1;
                    foreach (var row in Data.Rows)
                    {
                        topRow += row.Height;
                        if (Math.Abs(topRow - y) < 5)
                        {
                            changeKind = ChangeKind.RowResize;
                            changeRowIndex = index;
                        }
                        index++;
                    }
                }
            }
            else
                changeKind = null;
            /// column resize
            if (changeKind == ChangeKind.ColumnResize)
            {
                xStart = (int)x;
                leftStart = (int)Data.Left;
                if (changeColIndex.Value == 1) // first column
                    rightCellStartWidth = Data.HeaderCells.ElementAt(changeColIndex.Value - 1).Width;
                else if (changeColIndex.Value == Data.HeaderCells.Count + 1)
                {
                    //last column
                    leftCellStartWidth = Data.HeaderCells.ElementAt(changeColIndex.Value - 2).Width;
                }
                else
                {
                    //others columns
                    leftCellStartWidth = Data.HeaderCells.ElementAt(changeColIndex.Value - 2).Width;
                    rightCellStartWidth = Data.HeaderCells.ElementAt(changeColIndex.Value - 1).Width;
                }
                Bound.DisableSelection();
            }
            else if (changeKind == ChangeKind.RowResize)
            {
                yStart = (int)y;
                heightStart = Data.Rows.ElementAt(changeRowIndex.Value - 1).Height;
                Bound.DisableSelection();

            }
            statePushed = false;
        }

        public void Drag(double x, double y)
        {
            if (changeKind == null)
                return;
            if (!statePushed)
            {
                Bound.Page.Stack.Push(Data);
                statePushed = true;
            }
            if (changeKind == ChangeKind.ColumnResize)
            {
                int left;
                Bound.ShowRuler(this, (int)x, out left);
                var dif = xStart.Value - left;
                dif += difrentCurcer.Value;
                if (changeColIndex.Value == 1)
                {
                    Data.HeaderCells.ElementAt(changeColIndex.Value - 1).Width = rightCellStartWidth.Value + 2 * dif;
                    Data.Left = leftStart.Value - dif;
                }
                else if (changeColIndex.Value == Data.HeaderCells.Count + 1)
                {
                    Data.HeaderCells.ElementAt(changeColIndex.Value - 2).Width = leftCellStartWidth.Value - 2 * dif;
                    Data.Left = leftStart.Value + dif;
                }
                else
                {
                    Data.HeaderCells.ElementAt(changeColIndex.Value - 1).Width = rightCellStartWidth.Value + dif;
                    Data.HeaderCells.ElementAt(changeColIndex.Value - 2).Width = leftCellStartWidth.Value - dif;
                }
            }
            else
            {
                var dif = yStart.Value - (int)y;
                Data.Rows.ElementAt(changeRowIndex.Value - 1).Height = heightStart.Value - dif;
            }
        }

        void UpdateTableLocation()
        {
            if (Data.BondType.HasValue)
            {
                Data.Left = Convert.ToInt32(Bound.Left + (BoundItem.ColumnWidth - TableWidth - 15) / 2);
                Data.Top = Convert.ToInt32(BoundItem.Top);
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                UpdateTableLocation();
                StateHasChanged();
            }
            if (message != null)
            {
                await JSRuntime.InvokeVoidAsync("$.caspian.showMessage", message);
                message = null;
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        int TableWidth
        {
            get
            {
                var total = 15;
                foreach (var cell in Data.HeaderCells)
                    total += cell.Width;
                return total;
            }
        }
    }
}
