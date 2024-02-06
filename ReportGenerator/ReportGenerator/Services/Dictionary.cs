using System.Xml.Linq;

namespace ReportGenerator.Services
{
    public class Dictionary
    {
        IServiceProvider provider;

        public Dictionary(IServiceProvider provider) 
        {
            this.provider = provider;
        }

        public async Task<XElement> GetXmlElement(int reportId)
        {
            var dic = new XElement("Dictionary").AddAttribute("Ref", 10).AddAttribute("type", "Dictionary")
                .AddAttribute("isKey", true);
            var business = await new BusinessObject(provider).GetXmlElement(reportId);
            dic.AddElement(business);
            dic.AddElement("Databases").AddAttribute("isList", true).AddAttribute("count", 0);
            dic.AddElement("DataSources").AddAttribute("isList", true).AddAttribute("count", 0);
            dic.AddElement("Relations").AddAttribute("isList", true).AddAttribute("count", 0);
            dic.AddElement("Report").AddAttribute("isRef", 0);
            dic.AddElement("Resources").AddAttribute("isList", true).AddAttribute("count", 0);
            dic.AddElement("Variables").AddAttribute("isList", true).AddAttribute("count", 0);
            var variables = dic.AddElement("Variables").AddAttribute("isList", true).AddAttribute("count", 5).Element("Variables");
            variables.AddElement(GetVariable("ReportDate", typeof(DateTime))).AddElement(GetVariable("FirstName"))
                .AddElement(GetVariable("LastName")).AddElement(GetVariable("FullName")).AddElement(GetVariable("PersonalCode"));
            return dic;
        }

        private XElement GetVariable(string name, Type type = null)
        {
            type = type ?? typeof(string);
            var value = $",{name},{name},{type.Name},,False,False";
            return new XElement("value", value);
        }
    }
}
