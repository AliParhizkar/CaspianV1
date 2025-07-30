using Caspian.Common;
using Caspian.Report;
using System.Xml.Linq;
using Caspian.Report.Data;

namespace ReportGenerator.Services
{
    public static class Extension
    {
        public static string GetXMLElement(this Caspian.Report.Data.Color color)
        {
            if (color.ColorString == null || color.ColorString.ToLower() == "transparent" || color.ColorString.ToLower().StartsWith("rgba"))
                return "Transparent";
            return $"[{color.Red}:{color.Green}:{color.Blue}]";
        }

        public static int Id { get; private set; } = 30;

        public static int CreateId()
        {
            Id++;
            return Id;
        }

        public static string ToCentimeter(this double value)
        {
            var result = value / ReportComponentExtension.PPC;
            return Math.Round(result, 2).ToString().Replace('/', '.');
        }

        public static string ToCentimeter(this int value)
        {
            var result = value / ReportComponentExtension.PPC;
            return Math.Round(result, 2).ToString().Replace('/', '.');
        }

        public static XElement ClientRectangle(double left, double top, double width, double height)
        {
            return new XElement("ClientRectangle", $"{left.ToCentimeter()},{top.ToCentimeter()},{width.ToCentimeter()},{height.ToCentimeter()}");
        }

        public static byte? GetDataLevel(this BoundItemData BoundItemData)
        {
            if (BoundItemData.BondType < BondType.FirstDataLevel || BoundItemData.BondType > BondType.ThirdDataLevel)
                return null;
            return (byte)(BoundItemData.BondType.ConvertToInt() - 2);
        }
    }
}
