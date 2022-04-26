using System;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;
using Caspian.Common.Extension;

namespace ReportUiModels
{
    public class TableControl : ReportObject
    {
        public TableControl()
        {

        }

        public TableControl(XElement element)
        {
            Cells = new List<TableCell>();
            ColumnsCount = Convert.ToInt32(element.Element("ColumnCount").Value);
            DataLevel = Convert.ToInt32(element.Attribute("DataLevel").Value);
            RowsCount = Convert.ToInt32(element.Element("RowCount").Value);
            var components = element.Element("Components").Nodes().Where(t => t.NodeType != System.Xml.XmlNodeType.Text);
            foreach (var component in components)
                Cells.Add(new TableCell(component as XElement));
            for (var i = 0; i < Cells.Count; i++)
            {
                if (Cells[i].JoinWidth.GetValueOrDefault(1) > 1)
                {
                    var index = i - Cells[i].JoinWidth.Value + 1;
                    Cells[index].ColSpan = Cells[i].JoinWidth;
                    Cells[index].Enable = true;
                    Cells[i].Enable = false;
                }
                if (Cells[i].JoinHeight.GetValueOrDefault(1) > 1)
                {
                    var index = i - (Cells[i].JoinHeight.Value - 1) * ColumnsCount;
                    Cells[index].RowSpan = Cells[i].JoinHeight;
                    Cells[index].Enable = true;
                    Cells[i].Enable = false;
                }
            }
        }

        public int? DataLevel { get; set; }

        public Bond Bond { get; set; }

        public ReportPrintPage Page { get; set; }

        public Position Position { get; set; }

        public int RowsCount { get; set; }

        public int ColumnsCount { get; set; }

        public IList<TableCell> Cells { get; set; }

        private void UpdateCellsForRowSpan()
        {
            for (var i = 0; i < Cells.Count; i++)
            {
                if (Cells[i].RowSpan > 1)
                {
                    Cells[i].JoinCells.Clear();
                    var index = i + (Cells[i].RowSpan.Value - 1) * ColumnsCount;
                    for (var j = 1; j <= Cells[i].RowSpan; j++)
                    {
                        Cells[i + (j - 1) * ColumnsCount].Enable = false;
                        Cells[i + (j - 1) * ColumnsCount].ParentJoin = Cells[index].Id;
                        Cells[index].JoinCells.Add(Cells[i + (j - 1) * ColumnsCount].Id);
                    }
                    Cells[index].BackGroundColor = Cells[i].BackGroundColor;
                    Cells[index].Border = Cells[i].Border;
                    Cells[index].Color = Cells[i].Color;
                    Cells[index].JoinWidth = Cells[i].ColSpan;
                    Cells[index].Enable = true;
                    Cells[index].Font = Cells[i].Font;
                    Cells[index].HorizontalAlign = Cells[i].HorizontalAlign;
                    var temp = Cells[i].Position;
                    Cells[i].Position = Cells[index].Position;
                    Cells[index].Position = temp;
                    Cells[index].JoinHeight = Cells[i].RowSpan;
                    for (int j = 1; j < Cells[i].RowSpan.Value; j++)
                    {
                        Cells[i + (j - 1) * ColumnsCount].Position.Top = Cells[index].Position.Top;
                        Cells[i + (j - 1) * ColumnsCount].Position.Height = Cells[index].Position.Height / Cells[index].JoinHeight.Value;
                    }
                    Cells[index].Member = Cells[i].Member;
                    Cells[index].VerticalAlign = Cells[i].VerticalAlign;
                    Cells[index].Text = Cells[i].Text;
                }
            }
        }

        private void UpdateCellsForColumnSpan()
        {
            for (var i = 0; i < Cells.Count; i++)
            {
                if (Cells[i].ColSpan > 1)
                {
                    Cells[i].JoinCells.Clear();
                    var index = i + Cells[i].ColSpan.Value - 1;
                    for (var j = 1; j <= Cells[i].ColSpan; j++)
                    {
                        Cells[i + j - 1].Enable = false;
                        Cells[i + j - 1].ParentJoin = Cells[index].Id;
                        Cells[index].JoinCells.Add(Cells[i + j - 1].Id);
                    }
                    Cells[index].BackGroundColor = Cells[i].BackGroundColor;
                    Cells[index].RowSpan = Cells[i].RowSpan;
                    Cells[index].Border = Cells[i].Border;
                    Cells[index].Color = Cells[i].Color;
                    Cells[index].Enable = true;
                    Cells[index].Font = Cells[i].Font;
                    Cells[index].HorizontalAlign = Cells[i].HorizontalAlign;
                    var temp = Cells[i].Position;
                    Cells[i].Position = Cells[index].Position;
                    Cells[index].Position = temp;
                    Cells[index].Position.Width = temp.Width;
                    Cells[index].JoinWidth = Cells[i].ColSpan;
                    for (int j = 1; j < Cells[i].ColSpan.Value; j++)
                    {
                        //Cells[i + j - 1].Position.Left = Cells[index].Position.Left;
                        //Cells[i + j - 1].Position.Width = Cells[index].Position.Width / Cells[index].JoinWidth.Value;
                    }
                    Cells[index].Member = Cells[i].Member;
                    Cells[index].VerticalAlign = Cells[i].VerticalAlign;
                    Cells[index].Text = Cells[i].Text;
                }
            }
        }

        public override XElement GetXmlElement(ReportType reportType)
        {
            var element = new XElement("Table" + Id).AddAttribute("Ref", Id).AddAttribute("type", "Stimulsoft.Report.Components.Table.StiTable")
                .AddAttribute("isKey", true).AddAttribute("DataLevel", DataLevel.GetValueOrDefault(0));
            element.AddElement("Brush").AddContent("Transparent");
            int? count = null;
            if (reportType == ReportType.Report)
                count = ColumnsCount - 1;
            else
                count = ColumnsCount;
            var ee = new XElement("ClientRectangle", "0,0,21.005,0.771");
            element.AddElement(ee);
            element.AddElement("ColumnCount").AddContent(count);
            count = null;
            if (reportType == ReportType.Report)
                count = Cells.Count(t => t.CellType == CellType.Cell);
            else
                count = Cells.Count;
            element.AddElement("Components").AddAttribute("isList", true).AddAttribute("count", count);
            foreach (var cell in Cells)
            {
                cell.RowSpan = cell.RowSpan.GetValueOrDefault(1);
                cell.ColSpan = cell.ColSpan.GetValueOrDefault(1);
                if (!cell.Enable)
                {
                    cell.RowSpan = 1;
                    cell.ColSpan = 1;
                }
                cell.Table = this;
            }
            UpdateCellsForColumnSpan();
            UpdateCellsForRowSpan();
            if (Bond != null && Bond.BondType == BondType.DataHeader && reportType == ReportType.Report)
            {
                var headerHeight = Cells.First(t => t.CellType == CellType.ColumnHeader).Position.Height;
                var cells = Cells.Where(t => t.CellType == CellType.Cell);
                //foreach (var cell in cells)
                //    cell.Position.Top -= headerHeight;
            }
            foreach (var cell in Cells)
            {
                if (cell.CellType == CellType.Cell || reportType == ReportType.View)
                    element.Element("Components").Add(cell.GetXmlElement(reportType));
            }
            element.AddElement("Conditions").AddAttribute("isList", true).AddAttribute("count", 0);
            element.AddElement("Filters").AddAttribute("isList", true).AddAttribute("count", 0);
            element.AddElement("MinHeight").AddContent(0.2);
            element.AddElement("Name").AddContent("Table" + Id);
            element.AddElement("NumberID").AddContent(25);
            if (Page != null)
            {
                element.AddElement("BusinessObjectGuid").AddContent(Page.Report.Dictionary.GetGuId(DataLevel.Value));
                Bond masterBond = null;
                foreach (var bond in Page.Bonds)
                {
                    if (bond.DataLevel == DataLevel + 1)
                        masterBond = bond;
                }
                if (masterBond != null)
                    element.AddElement("MasterComponent").AddAttribute("isRef", masterBond.Id);
            }
            if (Bond == null)
            {
                element.AddElement("Page").AddAttribute("isRef", Page.Id);
                element.AddElement("Parent").AddAttribute("isRef", Page.Id);
            }
            else
            {
                element.AddElement("Page").AddAttribute("isRef", Bond.Page.Id);
                element.AddElement("Parent").AddAttribute("isRef", Bond.Id);
            }
            element.AddElement("RowCount").AddContent(RowsCount);
            element.AddElement("Sort").AddAttribute("isList", true).AddAttribute("count", 0);
            return element;
        }

        public override string GetJson()
        {
            var str = "{rowsCount:" + RowsCount + ",columnsCount:" + ColumnsCount;
            if (DataLevel.HasValue)
                str += ",dataLevel:" + DataLevel.Value;
            if (Position != null)
                str += ",position:" + Position.GetJson();
            str += ",cells:[";
            var firstTime = true;
            foreach (var cell in Cells)
            {
                if (!firstTime)
                    str += ",";
                str += cell.GetJson();
                firstTime = false;
            }
            str += "]";
            str += "}";
            return str;
        }
    }
}