using System;
using System.Xml;
using System.Linq;
using Caspian.Engine;
using Caspian.Common;
using System.Xml.Linq;
using Caspian.Common.Extension;
using System.Collections.Generic;

namespace ReportUiModels
{
    public class ReportPrint 
    {
        public ReportPrint()
        {

        }

        public ReportPrint(XmlElement element)
        {
            
        }

        public ReportPrint(Type type)
        {
            Dictionary = new Dictionary(type);
            Pages = new List<ReportPrintPage>();
        }

        public SubReportLevel? SubReportLevel { get; set; }

        public Dictionary Dictionary { get; set; }

        public IList<ReportPrintPage> Pages { get; set; }

        private string GetParameterName(string parameter, int level, int maxLevel, Type type)
        {
            if (level > 1)
            {
                var index = parameter.IndexOf('.');
                parameter = parameter.Substring(index + 1);
            }
            if (level > 2)
            {
                var index = parameter.IndexOf('.');
                parameter = parameter.Substring(index + 1);
            }
            parameter = parameter.Replace('.', '_');
            if (level == maxLevel)
                return "{list." + parameter + "}";
            var info1 = type.GetProperties().Single(t => t.PropertyType.IsGenericType && !t.PropertyType.IsNullableType());
            if (maxLevel == 2)
                return "{list." + info1.Name + '.' + parameter + '}';
            throw new Exception("خطا:این حالت پیش بینی نشده است.");
        }

        public void ClearParameterInPage(string parameter, int level, int maxLevel, Type type,XDocument doc)
        {
            var pages = doc.Element("StiSerializer").Element("Pages").Elements();
            foreach (var page in pages)
            {
                var bonds = page.Element("Components").Elements();
                foreach (var bond in bonds)
                {
                    if (bond.Name == "DataBand1")
                        bond.Element("BusinessObjectGuid").Value = Dictionary.GetGuId(1);
                    else
                        if (bond.Name == "DataBand2")
                            bond.Element("BusinessObjectGuid").Value = Dictionary.GetGuId(2);
                        else
                            if (bond.Name == "DataBand3")
                                bond.Element("BusinessObjectGuid").Value = Dictionary.GetGuId(3);
                    var controls = bond.Element("Components").Elements();
                    foreach (var control in controls)
                    {
                        if (control.Name.LocalName.StartsWith("Table"))
                        {
                            var parameterName = GetParameterName(parameter, level, maxLevel, type);
                            var cells = control.Element("Components").Elements();
                            foreach(var cell in cells)
                            {
                                var text = cell.Element("Text");
                                if (text != null && text.Value.HasValue() && parameterName == text.Value)
                                        text.Value = "حذف پارامتر";
                            }
                        }
                        else
                        {
                            var text = control.Element("Text");
                            if (text != null && text.Value.HasValue())
                            {
                                var parameterName = GetParameterName(parameter, level, maxLevel, type);
                                if (parameterName == text.Value)
                                {
                                    text.Value = "حذف پارامتر";
                                }
                            }
                        }
                    }
                }
            }
        }

        public XElement GetXmlElement(ReportType reportType)
        {
            var element = new XElement("StiSerializer").AddAttribute("version", 1.02).AddAttribute("type", "Flex").AddAttribute("application", "StiReport");
            element.Add(Dictionary.GetXmlElement(reportType));
            element.AddElement("EngineVersion").AddContent("EngineV2");
            element.AddElement("GlobalizationStrings").AddAttribute("isList", true).AddAttribute("count", 0);
            element.AddElement("MetaTags").AddAttribute("isList", true).AddAttribute("count", 0);
            var pages = new XElement("Pages").AddAttribute("isList", true).AddAttribute("count", Pages.Count);
            foreach (var page in Pages)
            {
                page.Report = this;
                pages.Add(page.GetXmlElement(reportType));
            }
            element.Add(pages);
            element.AddElement("PrinterSettings").AddAttribute("Ref", 9999).AddAttribute("type", "Stimulsoft.Report.Print.StiPrinterSettings").AddAttribute("isKey", true);
            element.AddElement("ReportAlias").AddContent("Report");
            element.AddElement("ReportChanged").AddContent("5/3/2015 4:05:02 PM");
            element.AddElement("ReportCreated").AddContent("5/1/2015 7:31:06 PM");
            element.AddElement("ReportFile").AddContent("D:\\e.mrt");
            element.AddElement("ReportGuid").AddContent("7a59e6f3323042d4848703beddfe3e7b");
            element.AddElement("ReportName").AddContent("ReportName");
            element.AddElement("ReportUnit").AddContent("Centimeters");
            element.AddElement("ReportVersion").AddContent("2012.1.1300");
            element.AddElement("ScriptLanguage").AddContent("CSharp");
            element.AddElement("Styles").AddAttribute("isList", true).AddAttribute("count", 0);
            return element;
        }

        public string GetJson()
        {
            var str = "{";
            if (SubReportLevel.HasValue)
                str += "subReportLevel:" + SubReportLevel.Value.ConvertToInt();
            str += "}";
            return str;
        }
    }
}