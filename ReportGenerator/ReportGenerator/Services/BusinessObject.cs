using Caspian.Engine;
using Caspian.Common;
using System.Xml.Linq;
using Caspian.Engine.Service;
using Caspian.Common.Extension;
using Microsoft.EntityFrameworkCore;

namespace ReportGenerator.Services
{
    public class BusinessObject
    {
        IServiceProvider provider;
        static IDictionary<int, string> GuIds;
        static IDictionary<int, string> BusinessObjectsPath;
        public BusinessObject(IServiceProvider provider) 
        {
            this.provider = provider;
            GuIds = new Dictionary<int, string>();
            BusinessObjectsPath = new Dictionary<int, string>();
        }

        public static string GetGuId(int dataLevel)
        {
            return GuIds[dataLevel];
        }

        public static string GetBusinessObjectsPath(int dataLevel)
        {
            var name = "";
            for (var i = 1; i <= dataLevel; i++)
                name += $"{BusinessObjectsPath[i]}.";
            return name;
        }

        public async Task<XElement> GetXmlElement(int reportId)
        {
            var report = await provider.GetService<ReportService>().GetAll().Include(t => t.ReportParams).Include(t => t.ReportGroup).SingleAsync(reportId);
            var mainType = new AssemblyInfo().GetReturnType(report.ReportGroup);
            var selectReport = new SelectReport(mainType);
            var type = selectReport.GetEqualType(report.ReportParams.ToList());
            type = new ReportPrintEngine(provider).GetTypeOf(report.ReportParams.ToList(), type, mainType.Name);
            return GetXElement(type, "list", 11);
        }

        public XElement GetXElement(Type type, string name, int id) 
        {
            var detailsInfo = type.GetProperties().SingleOrDefault(t => t.PropertyType.IsCollectionType() && 
                t.PropertyType != typeof(string));
            var node = new XElement("BusinessObjects").AddAttribute("isList", true).AddAttribute("count", 1);
            var listNode = node.AddElement(name).AddAttribute("Ref", id).AddAttribute("type", "Stimulsoft.Report.Dictionary.StiBusinessObject")
                .AddAttribute("isKey", true).Element(name);
            listNode.AddElement("Alias", name);
            if (detailsInfo == null)
                listNode.AddElement(new XElement("BusinessObjects").AddAttribute("isList", true).AddAttribute("count", 0));
            else
                listNode.AddElement(GetXElement(detailsInfo.PropertyType.GetGenericArguments()[0], detailsInfo.Name, id + 1));
            listNode.AddElement("Category");
            var columnsInfo = type.GetProperties().Where(t => (!t.PropertyType.IsCollectionType() || t.PropertyType == typeof(string)) && t.IsCollectible);
            var columnsNode =  listNode.AddElement("Columns").AddAttribute("isList", true)
                .AddAttribute ("count", columnsInfo.Count()).Element("Columns");
            foreach ( var column in columnsInfo)
            {
                string content = $"{column.Name.Replace(".", "_")},{column.PropertyType.Namespace}.{column.PropertyType.Name}";
                columnsNode.AddElement("Value", content);
            }
            listNode.AddElement("Dictionary").AddAttribute("isRef", 10);
            var guId = Guid.NewGuid().ToString().Replace("-", "");
            listNode.AddElement("Guid", guId);
            GuIds.Add(id - 10, guId);
            BusinessObjectsPath.Add(id - 10, name);
            listNode.AddElement("Name", name);
            return node;
        }
    }
}
