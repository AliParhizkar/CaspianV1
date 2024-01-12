using Caspian.Common;
using System.Xml;
using System.Xml.Linq;

namespace Caspian.Server
{
    public class Bond: Common.Bond
    {
        public Bond(XElement element) 
        {
            var type = element.Attribute("type").Value;
            var isTable = false;
            switch (type)
            {
                case "ReportTitleBand":
                    BondType = BondType.ReportTitle;
                    break;
                case "PageHeaderBand":
                    BondType = BondType.PageHeader;
                    break;
                case "PageFooterBand":
                    BondType = BondType.PageFooter;
                    break;
                case "HeaderBand":
                case "Stimulsoft.Report.Components.StiColumnHeaderBand":
                    BondType = BondType.DataHeader;
                    break;
                case "DataBand":
                    BondType = BondType.DataBond;
                    break;
                case "FooterBand":
                    BondType = BondType.DataFooter;
                    break;
                case "Stimulsoft.Report.Components.Table.StiTable":
                    isTable = true;
                    BondType = BondType.DataBond;
                    break;
            }
            var columnsElement = element.Element("Columns");
            if (columnsElement != null)
                ColumnsCount = (ColumnCountType)Convert.ToInt32(columnsElement.Value);
            if (isTable)
            {
                //Table = new TableControl(element);
                Height = Table.Cells.First().Position.Height;
            }
            else
            {
                var dataLevelElement = element.Element("DataLevel");
                if (dataLevelElement != null)
                    DataLevel = Convert.ToInt32(dataLevelElement.Value);
                //PrintOn = GetPrintOn(element.Element("PrintOn"));
                BackGroundColor = new Color(element.Element("Brush").Value);
                var components = element.Element("Components").Nodes().Where(t => t.NodeType != XmlNodeType.Text);
                //var position = new Position(element.Element("ClientRectangle"));
                if (BondType == BondType.DataBond)
                {
                    if (element.Element("NewPageBefore") != null)
                        NewPageBefore = Convert.ToBoolean(element.Element("NewPageBefore").Value.ToLower());
                    if (element.Element("NewPageAfter") != null)
                        NewPageAfter = Convert.ToBoolean(element.Element("NewPageAfter").Value.ToLower());
                }
                //Height = position.Height;
                Controls = new List<ReportPrintControl>();
                foreach (var component in components)
                {
                    var temp = component as XElement;
                    var controlType = temp.Attribute("type").Value;
                    //if (controlType == "Stimulsoft.Report.Components.Table.StiTable")
                    //    Table = new TableControl(temp);
                    //else
                    //    Controls.Add(new ReportPrintControl(temp));
                }
            }
        }
    }
}
