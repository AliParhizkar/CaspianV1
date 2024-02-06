using System.Xml.Linq;
using Caspian.Common.Extension;

namespace ReportUiModels
{
    public class Dictionary: ReportObject
    {
        public Dictionary(Type type)
        {
            BusinessObject = new BusinessObject();
            InitBusinessObject(type, this.BusinessObject, "List");
        }

        public string GetGuId(int level)
        {
            var maxDataLevel = 1;
            if (BusinessObject.SubBusinessObject != null)
            {
                if (BusinessObject.SubBusinessObject.SubBusinessObject != null)
                    maxDataLevel = 3;
                else
                    maxDataLevel = 2;
            }
            if (level == maxDataLevel)
                return BusinessObject.Guid;
            if (level == maxDataLevel - 1)
                return BusinessObject.SubBusinessObject.Guid;
            if (level == maxDataLevel - 2)
                return BusinessObject.SubBusinessObject.SubBusinessObject.Guid;
            throw new Exception("خطا:Level is invalid");
        }

        void InitBusinessObject(Type type, BusinessObject businessObject, string name)
        {
            foreach (var info in type.GetProperties())
            {
                if (info.PropertyType.IsGenericType && !info.PropertyType.IsValueType)
                {
                    businessObject.SubBusinessObject = new BusinessObject();
                    InitBusinessObject(info.PropertyType.GenericTypeArguments[0], businessObject.SubBusinessObject, info.Name);
                }
                else
                    businessObject.Columns.Add(new Column(info.Name, info.PropertyType));
            }
            businessObject.Dictionary = this;
            businessObject.Name = name;
        }

        public BusinessObject BusinessObject { get; private set; }

        public override XElement GetXmlElement(ReportType reportType)
        {
            var count = 0;
            if (BusinessObject != null)
                count = 1;
            var dictionary = new XElement("Dictionary").AddAttribute("Ref", Id).AddAttribute("type", "Dictionary").AddAttribute("isKey", true);
            var businessObjects = new XElement("BusinessObjects").AddAttribute("isList", true).AddAttribute("count", count);
            if (count > 0)
                businessObjects.Add(BusinessObject.GetXmlElement(reportType));
            dictionary.Add(businessObjects);
            dictionary.AddElement("Databases").AddAttribute("isList", true).AddAttribute("count", 0);
            dictionary.AddElement("DataSources").AddAttribute("isList", true).AddAttribute("count", 0);
            dictionary.AddElement("Relations").AddAttribute("isList", true).AddAttribute("count", 0);
            dictionary.AddElement("Report").AddAttribute("isRef", 0);
            var variablesElements = dictionary.AddElement("Variables").AddAttribute("isList", true).AddAttribute("count", 6).Element("Variables");
            variablesElements.AddElement(GetVariable("Date")).AddElement(GetVariable("FName")).AddElement(GetVariable("LName"))
                .AddElement(GetVariable("FLName")).AddElement(GetVariable("UserId")).AddElement(GetVariable("NatinalCode"));
            return dictionary;
        }

        private XElement GetVariable(string name, Type type = null)
        {
            var value = "," + name + "," + name + ",";
            if (type == typeof(string) || type == null)
                value += "System.String";
            else if (type == typeof(Int32))
                value += "System.Int32";
            else
                throw new Exception("خطای عدم پیش بینی");
            value += ",,False,False";
            return new XElement("value", value);
        }
    }

    public class BusinessObject: ReportObject
    {
        public BusinessObject()
        {
            Columns = new List<Column>();
            Guid = System.Guid.NewGuid().ToString().Replace("-", "");
        }

        public Dictionary Dictionary { get; set; }

        public string Guid { get; private set; }

        public string Name { get; set; }

        public BusinessObject SubBusinessObject { get; set; }

        public IList<Column> Columns { get; private set; }

        public override XElement GetXmlElement(ReportType reportType)
        {
            var element = new XElement(Name).AddAttribute("Ref", Id).AddAttribute("type", "Stimulsoft.Report.Dictionary.StiBusinessObject").AddAttribute("isKey", true);
            element.AddElement("Alias").AddContent(Name);
            var count = 0;
            if (SubBusinessObject != null)
                count = 1;
            element.AddElement("BusinessObjects").AddAttribute("isList", true).AddAttribute("count", count);
            if (count > 0)
                element.Element("BusinessObjects").Add(SubBusinessObject.GetXmlElement(reportType));
            element.AddElement("Category");
            var columns = new XElement("Columns").AddAttribute("isList", true).AddAttribute("count", Columns.Count);
            foreach (var column in Columns)
                columns.Add(column.GetXmlElement());
            element.Add(columns);
            element.Add(new XElement("Dictionary").AddAttribute("isRef", Dictionary.Id));
            element.Add(new XElement("Guid").AddContent(Guid));
            element.Add(new XElement("Name").AddContent(Name));
            return element;
        }
    }

    public class Column
    {
        public Column(string name, Type type)
        {
            Name = name;
            Type = type;
        }

        public string Name { get; private set; }

        public Type Type { get; private set; }

        public XElement GetXmlElement()
        {
            return new XElement("value").AddContent(Name + "," + Type.FullName);
        }
    }
}