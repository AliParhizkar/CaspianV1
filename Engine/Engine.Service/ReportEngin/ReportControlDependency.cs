using System.Xml.Linq;
using Caspian.Common.Extension;

namespace Caspian.Engine
{
    public class ReportControlDependency
    {
        public ReportControlDependency()
        {

        }

        public ReportControlDependency(XElement element)
        {
            Id = Convert.ToInt32(element.Attribute("Id").Value);
            DependControlId = Convert.ToInt32(element.Attribute("DependControlId").Value);
            Dependency = element.Element("Dependency").Value;
        }

        public int Id { get; set; }

        public int ControlId { get; set; }

        public int DependControlId { get; set; }

        public string Dependency { get; set; }

        public XElement GetXml()
        {
            var element = new XElement("Dependent").AddAttribute("Id", Id)
                .AddAttribute("DependControlId", DependControlId).AddElement("Dependency", Dependency);
            return element;
        }
    }
}
