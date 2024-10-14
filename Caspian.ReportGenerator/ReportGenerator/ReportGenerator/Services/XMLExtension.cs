using System.Xml.Linq;

namespace ReportGenerator.Services
{
    public static class XMLExtension
    {
        public static XElement AddElement(this XElement element, string name)
        {
            element.Add(new XElement(name));
            return element;
        }

        public static XElement AddElement(this XElement element, XElement newElement)
        {
            element.Add(newElement);
            return element;
        }

        public static XElement AddElement(this XElement element, string name, object value)
        {
            element.Add(new XElement(name, value));
            return element;
        }

        public static XElement AddAttribute(this XElement element, string name, object value)
        {
            XElement temp = null;
            if (element.LastNode == null)
                temp = element;
            else
                temp = element.LastNode as XElement;
            if (value != null)
                temp.Add(new XAttribute(name, value));
            return element;
        }

        public static XElement AddContent(this XElement element, object value)
        {
            XElement temp = null;
            if (element.LastNode == null)
                temp = element;
            else
                temp = element.LastNode as XElement;
            temp.Add(value);
            return element;
        }
    }
}
