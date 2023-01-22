using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Caspian.common
{
    public class RazorPageProceccor
    {
        public void Proccess()
        {
            var str = File.ReadAllText("F:\\a.xml");
            str = str.Replace("@bind-", "bind-").Replace("@ref", "ref");
            str = ClearComment(str);
            var doc = XDocument.Parse(str, LoadOptions.None);
            FindElement(doc.Root);
        }

        void FindElement(XNode node)
        {
            switch (node.NodeType)
            {
                case XmlNodeType.Element:
                    var element = node as XElement;
                    switch (element.Name.LocalName)
                    {
                        case "StringTextBox":

                            break;
                        case "DataGrid":
                            FindGridData(element);
                            break;
                        default:
                            foreach(var item in element.Nodes())
                            {
                                FindElement(item);
                            }
                            break;
                    }
                    break;
                default:
                    if (node.NodeType != XmlNodeType.Text)
                        throw new NotImplementedException("خطای عدم پیاده سازی");
                    break;
            }
            
        }

        void FindGridData(XElement gElement)
        {
            var typeName = gElement.Attribute("TEntity");
            var columns =  gElement.Nodes().SingleOrDefault(t => t.NodeType == XmlNodeType.Element && (t as XElement).Name == "Columns");
            if (columns != null)
                FindColumnsField(columns);
        }

        void FindColumnsField(XNode node)
        {
            switch(node.NodeType)
            {
                case XmlNodeType.Element:
                    var element = node as XElement;
                    if (element.Name == "GridColumn")
                    {
                        var field = element.Attribute("Field");
                        if (field != null)
                        {
                            var value = field.Value;
                        }
                    }
                    else
                    {
                        foreach (var col in element.Nodes())
                            FindColumnsField(col);
                    }
                    break;
                default:
                    if (node.NodeType != XmlNodeType.Text)
                        throw new NotImplementedException("خطای عدم پیاده سازی");
                    break;
            }
        }


        string ClearComment(string str)
        {
            int index = str.IndexOf("@*");
            while (index != -1)
            {
                var index1 = str.IndexOf("*@") + 2;
                str = str.Substring(0, index) + str.Substring(index1);
                index = str.IndexOf("@*");
            }
            return str;
        }
    }
}
