using Caspian.Report;
using Caspian.Common;
using System.Xml.Linq;
using Caspian.Report.Data;
using Caspian.Common.Extension;

namespace ReportGenerator.Services
{
    public static class TableExtension
    {
        public static XElement GetXMLElement(this TableData data, PageData page)
        {
            string name;
            int id;
            double tableHeight;
            if (data.BondType == Caspian.Report.BondType.DataHeader)
            {
                name = "Table1";
                id = 16;
                tableHeight = data.Rows.Sum(t => t.Height);
            }
            else
            {
                tableHeight = page.Setting.PageHeight * page.PixelsPerCentimetre;
                name = "Table2";
                id = 17;
            }
            var element = new XElement(name).AddAttribute("Ref", id).AddAttribute("type", "Stimulsoft.Report.Components.Table.StiTable").AddAttribute("isKey", true);
            if (data.BondType > BondType.DataHeader && data.BondType < BondType.DataFooter)
            {
                var guId = BusinessObject.GetGuId(data.BondType.ConvertToInt().Value - 2);
                element.AddElement("BusinessObjectGuid", guId);
            }
            
            element.AddElement(data.Border.GetXMLElement());
            element.AddElement(Extension.ClientRectangle(0, 0, page.Width, tableHeight));
            element.AddElement("ColumnCount", data.HeaderCells.Count);
            var count = data.HeaderCells.Count() * data.Rows.Count();
            var cellsElement = element.AddElement("Components").AddAttribute("isList", true).AddAttribute("count", count).Element("Components");
            var index = 1;
            var top = 0;
            foreach (var row in data.Rows)
            {
                var left = data.Left - Bound.Left + 15;
                foreach (var cell in row.Cells)
                {
                    cell.Row = row;
                    var headerCell = data.HeaderCells[cell.ColIndex - 1];
                    ///For Colspan > 1
                    var width = headerCell.Width;
                    for (var i = 1; i < cell.ColSpan; i++)
                        width += data.HeaderCells[cell.ColIndex - 1 + i].Width;
                    /// For Rowspan > 1 
                    if (cell.RowSpan > 1)
                    {
                        for (var i = row.RowIndex; i < cell.RowSpan + row.RowIndex; i++)
                        {
                            var lastRowCellIndex = (cell.RowSpan - 1) * data.HeaderCells.Count() + index - 1;
                            // Cell on last row is parentjoin cell
                            data.Rows[i - 1].Cells[cell.ColIndex - 1].ParentJoin = lastRowCellIndex;
                        }
                    }

                    var height = row.Height;
                    if (cell.ParentJoin == index - 1)
                    {
                        var preRow = data.Rows[row.RowIndex - 2];
                        while (preRow.Cells[cell.ColIndex - 1].ParentJoin.HasValue)
                        {
                            height += preRow.Height;
                            if (preRow.RowIndex < 2 || data.Rows[preRow.RowIndex - 2].Cells[cell.ColIndex - 1].ParentJoin is null)
                                break;
                            preRow = data.Rows[preRow.RowIndex - 2];
                        }
                        cell.CopyCellData(preRow.Cells[cell.ColIndex - 1]);
                    }
                    var xml = cell.GetXMLElement($"{name}_Cell{index}", left, top, width, height, data.BondType.Value);
                    cellsElement.Add(xml);
                    left += headerCell.Width;
                    index++;
                }
                top += row.Height;
            }
            var bond = page.Bound.Items.SingleOrDefault(t => t.Table != null && t.BondType != BondType.DataHeader);
            if (bond?.BondType == BondType.ThirdDataLevel)
                element.AddElement("MasterComponent").AddAttribute("isRef", 5);
            element.AddElement("Conditions").AddAttribute("isList", true).AddAttribute("count", 0);
            element.AddElement("DataRelationName").AddAttribute("isNull", true);
            element.AddElement("Expressions").AddAttribute("isList", true).AddAttribute("count", 0);
            element.AddElement("Filters").AddAttribute("isList", true).AddAttribute("count", 0);

            element.AddElement("Name", name);
            element.AddElement("NumberID", id + 2);
            element.AddElement("Page").AddAttribute("isRef", 15);
            ///If data bond is data header bond, Table's parent is data header bond else table's parent is page
            var parentId = data.BondType == BondType.DataHeader ? 3 : 15;
            element.AddElement("Parent").AddAttribute("isRef", parentId);
            element.AddElement("RowCount", data.Rows.Count);
            element.AddElement("Sort").AddAttribute("isList", true).AddAttribute("count", 0);
            return element;
        }
        
        public static XElement GetXMLElement(this TableCellData data, string cellName, int left, int top, int width, int height, BondType bondType)
        {
            var id = Extension.CreateId();
            var element = new XElement(cellName).AddAttribute("Ref", id).AddAttribute("type", "TableCell").AddAttribute("isKey", true);
            element.AddElement("Brush", data.BackgroundColor.GetXMLElement());
            element.AddElement(Extension.ClientRectangle(left, top, width, height));
            element.AddElement("Conditions").AddAttribute("isList", true).AddAttribute("count", 0);
            element.AddElement("Expressions").AddAttribute("isList", true).AddAttribute("count", 0);
            if (data.Hidden && data.ParentJoin is null)
                element.AddElement("Enabled", false);
            element.AddElement(data.Font.GetXMLElement());
            var ID = Convert.ToInt32(cellName.Substring(11)) - 1;
            if (data.ParentJoin.HasValue)
                element.AddElement("ParentJoin", data.ParentJoin);
            element.AddElement("ID", ID);
            if (data.ColSpan > 1)
            {
                element.AddElement("JoinCells").AddAttribute("isList", true).AddAttribute("count", data.ColSpan);
                for (var i = 0; i < data.ColSpan; i++)
                    element.Element("JoinCells").AddElement("value", ID + i);
            }
            else
                element.AddElement("JoinCells").AddAttribute("isList", true).AddAttribute("count", 0);
            element.AddElement("HorAlignment", data.Alignment.HorizontalAlign == HorizontalAlign.Justify ? "Width" : data.Alignment.HorizontalAlign.ToString());
            element.AddElement("VertAlignment", data.Alignment.VerticalAlign == VerticalAlign.Middle ? "Center" : data.Alignment.VerticalAlign.ToString());
            element.AddElement("TextBrush", data.Font.Color.GetXMLElement());
            element.AddElement(data.Border.GetXMLElement());
            element.AddElement("Margins", "0,0,0,0");
            element.AddElement("Name", cellName);
            element.AddElement("Page").AddAttribute("isRef", 15);
            element.AddElement("Parent").AddAttribute("isRef", bondType == BondType.DataHeader ? 16 : 17);
            string text = null;
            if (data.FieldData.SystemFiledType.HasValue)
                text = $"{{{data.FieldData.SystemFiledType}}}";
            if (data.FieldData.TotalFuncType.HasValue)
                text = $"{{{data.FieldData.TotalFuncType}}}";
            if (data.FieldData.SystemVariable.HasValue)
                text = $"{{{data.FieldData.SystemVariable}}}";
            if (data.FieldData.Path.HasValue())
            {
                var path = BusinessObject.GetBusinessObjectsPath(bondType.ConvertToInt().Value - 2);
                text = $"{{{path}{data.FieldData.Path.Replace('.', '_')}}}";
            }
            text ??= data.Text;
            element.AddElement("Text", text);
            return element;
        }

        public static void CopyCellData(this TableCellData data, TableCellData cell)
        {
            var row = data.Row;
            foreach (var info in typeof(TableCellData).GetProperties())
                info.SetValue(data, info.GetValue(cell));
            data.Row = row;
        }
    }
}
