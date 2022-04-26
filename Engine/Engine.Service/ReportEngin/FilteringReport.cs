using Caspian.Engine;
using System.Xml.Linq;
using Caspian.Common.Extension;

namespace Caspian.Engine
{
    public class FilteringReport
    {
        public FilteringReport()
        {
            Panels = new List<TabPanelModel>();
            Dependencies = new List<ReportControlDependency>();
        }

        public FilteringReport(Type type, XElement element)
        {
            Id = Convert.ToInt32(element.Attribute("Id").Value);
            Title = element.Attribute("Title").Value;
            var panels = element.Element("Panels").Elements();
            Panels = panels.Select(t => new TabPanelModel(type, t)).ToList();
            var dependencies = element.Element("Dependencies").Elements();
            Dependencies = dependencies.Select(t => new ReportControlDependency(t)).ToList();
        }

        public int Id { get; set; }

        public string Title { get; set; }

        public IList<TabPanelModel> Panels { get; set; }

        public IList<ReportControlDependency> Dependencies { get; set; }

        public XElement GetXml()
        {
            var report = new XElement("Report").AddAttribute("Id", Id).AddAttribute("Title", Title)
                .AddElement("Panels").AddElement("Dependencies");
            var panels = report.Element("Panels").AddAttribute("Count", Panels.Count);
            foreach(var panel in Panels)
                panels.AddElement(panel.GetXml());
            var dependencies = report.Element("Dependencies").AddAttribute("Count", Dependencies.Count);
            foreach (var dependency in Dependencies)
                dependencies.AddElement(dependency.GetXml());
            return report;
        }

        public string ToJson(Type type)
        {
            var str = "[";
            foreach (var panel in Panels)
            {
                if (str != "[")
                    str += ",";
                str += panel.ToJson(type);
            }
            str += "]";
            return str;
        }
    }
}