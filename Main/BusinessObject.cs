using Caspian.Engine;
using Caspian.Common;
using System.Xml.Linq;
using System.Reflection;
using Caspian.Engine.Model;
using Caspian.Engine.Service;
using Caspian.Common.Extension;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReportGenerator.Services
{
    public class BusinessObject
    {
        IServiceProvider provider;
        static IDictionary<int, string> GuIds;
        static IDictionary<int, string> BusinessObjectsPath;
        Type mainType;
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
            var report = await provider.GetCaspianService<ReportService>().GetAll().Include(t => t.ReportGroup).SingleAsync(reportId);
            mainType = new AssemblyInfo().GetReturnType(report.ReportGroup);
            if (report.ReportType == ReportType.Aggregate)
            {
                var groupParameters = await provider.GetCaspianService<AggregateReportGroupParameterService>().GetAll()
                    .Where(t => t.ReportGroupId == report.ReportGroupId).ToListAsync();
                var parameters = await provider.GetCaspianService<AggregateReportParameterService>().GetAll().Where(t => t.ReportId == reportId)
        .           Include(t => t.AggregateReportGroupParameter).ToListAsync();
                return GetXElement(parameters);
            }
            else
            {
                var parameters = await provider.GetCaspianService<ReportParamService>().GetAll().Where(t => t.ReportId == reportId)
                        .Include(t => t.ReportGroupParameter).ToListAsync();
                var maxLevel = parameters.Max(t => t.DataLevel);
                return GetXElement(parameters, maxLevel, "list", 11);
            }
        }

        Type GetParameterType(AggregateReportGroupParameter parameter, Type entityType, out string propertyPath)
        {
            var path = parameter.Path;
            if (parameter.AggregateFunctionType == null)
            {
                if (parameter.ParentParameter != null)
                {
                    var parentPath = parameter.ParentParameter.Path;
                    var property = entityType.GetMyProperty(parentPath);
                    if (property.PropertyType.GetUnderlyingType() == typeof(DateOnly))
                    {
                        var index = parentPath.LastIndexOf('.') + 1;
                        parentPath = parentPath.Substring(0, index);
                        var persianDateInfo = property.DeclaringType.GetProperties().Single(t => t.PropertyType == typeof(PersianDateTable) &&
                            t.GetCustomAttribute<ForeignKeyAttribute>()?.Name == property.Name);
                        propertyPath = $"{parentPath}.{persianDateInfo.Name}.{path}";
                        if (path == "Day" || path == "Year")
                            return typeof(int);
                        return typeof(string);
                    }
                }
                propertyPath = path;
                return entityType.GetMyProperty(path).PropertyType;
            }
            else
            {
                propertyPath = parameter.ParentParameter.Path + parameter.AggregateFunctionType.ToString();
                return entityType.GetMyProperty(parameter.ParentParameter.Path).PropertyType;
            }
        }

        public XElement GetXElement(IList<AggregateReportParameter> parameters)
        {
            var node = new XElement("BusinessObjects").AddAttribute("isList", true).AddAttribute("count", 1);
            var listNode = node.AddElement("list").AddAttribute("Ref", "11").AddAttribute("type", "Stimulsoft.Report.Dictionary.StiBusinessObject")
                .AddAttribute("isKey", true).Element("list");
            listNode.AddElement("Alias", "list");
            listNode.AddElement(new XElement("BusinessObjects").AddAttribute("isList", true).AddAttribute("count", 0));
            listNode.AddElement("Category");
            var columnsNode = listNode.AddElement("Columns").AddAttribute("isList", true)
                .AddAttribute("count", parameters.Count()).Element("Columns");
            foreach(var param in parameters.Select(t => t.AggregateReportGroupParameter))
            {
                string path = null;
                var type = GetParameterType(param, mainType, out path);
                if (path.HasValue())
                {
                    if (type.GetUnderlyingType().IsEnum)
                        type = typeof(string);
                    string content = $"{path.Replace(".", "")},{type.Namespace}.{type.GetUnderlyingType().Name}";
                    if (type.IsNullableType())
                        content += "?";
                    columnsNode.AddElement("Value", content);
                }
            }
            listNode.AddElement("Dictionary").AddAttribute("isRef", 10);
            var guId = Guid.NewGuid().ToString().Replace("-", "");
            listNode.AddElement("Guid", guId);
            GuIds.Add(1, guId);
            BusinessObjectsPath.Add(1, "list");
            listNode.AddElement("Name", "list");
            return node;
        }

        public XElement GetXElement(IList<ReportParam> reportParams, int level, string name, int id) 
        {
            var node = new XElement("BusinessObjects").AddAttribute("isList", true).AddAttribute("count", 1);
            var listNode = node.AddElement(name).AddAttribute("Ref", id).AddAttribute("type", "Stimulsoft.Report.Dictionary.StiBusinessObject")
                .AddAttribute("isKey", true).Element(name);
            listNode.AddElement("Alias", name);
            if (level == 1)
                listNode.AddElement(new XElement("BusinessObjects").AddAttribute("isList", true).AddAttribute("count", 0));
            else
            {
                listNode.AddElement(GetXElement(reportParams, level - 1, "__Details", id + 1));
            }
            listNode.AddElement("Category");
            var parameters = reportParams.Where(t => t.DataLevel == level).Select(t => t.ReportGroupParameter).ToList();
            var columnsNode =  listNode.AddElement("Columns").AddAttribute("isList", true)
                .AddAttribute ("count", parameters.Count()).Element("Columns");
            foreach ( var param in parameters)
            {
                var type = mainType.GetMyProperty(param.PropertyPath).PropertyType;
                if (type.GetUnderlyingType().IsEnum)
                    type = typeof(string);
                string content = $"{param.PropertyPath.Replace(".", "")},{type.Namespace}.{type.GetUnderlyingType().Name}";
                if (type.IsNullableType())
                    content += "?";
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
