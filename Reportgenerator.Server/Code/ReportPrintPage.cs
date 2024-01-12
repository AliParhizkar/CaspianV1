using System.Xml;
using System.Xml.Linq;

namespace Caspian.Server
{
    public class ReportPrintPage : Common.ReportPrintPage
    {
        public ReportPrintPage() { }

        public ReportPrintPage(XElement element) 
        {
            this.Border = new Border(element.Element("Border"));
            GuId = element.Element("Guid").Value;
            this.BackGroundColor = new Color(element.Element("Brush").Value);
            PageType = (Common.ReportPageType)Convert.ToInt32(element.Element("PageType").Value);
            var decimalDigits = Convert.ToChar(Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator);
            Width = Convert.ToDecimal(element.Element("PageWidth").Value.Replace('.', decimalDigits));
            Height = Convert.ToDecimal(element.Element("PageHeight").Value.Replace('.', decimalDigits));
            var bonds = element.Element("Components").Nodes().Where(t => t.NodeType != XmlNodeType.Text);
            //var test = bonds.Select(t => t.NodeType);
            Bonds = new List<Bond>();
            foreach (var item in bonds)
                (Bonds as IList<Bond>).Add(new Bond(item as XElement));
        }
    }
}
