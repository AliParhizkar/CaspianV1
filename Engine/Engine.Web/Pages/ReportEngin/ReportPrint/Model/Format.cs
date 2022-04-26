using Caspian.Common;
using Caspian.Common.Extension;
using System;
using System.Xml.Linq;

namespace ReportUiModels
{
    public class Format
    {
        public Format(XElement element)
        {
            DigitGroup = Convert.ToBoolean(element.Attribute("DigitGroup").Value);
            NumberDigits = Convert.ToInt32(element.Attribute("NumberDigits").Value);
            var groupNumberChar = element.Attribute("GroupNumberChar").Value;
            GroupNumberChar = groupNumberChar == "Dot" ? ReportUiModels.GroupNumberChar.Dot : ReportUiModels.GroupNumberChar.Camma;
            var decimalChar = element.Attribute("DecimalChar").Value;
            DecimalChar = decimalChar == "Slash" ? ReportUiModels.DecimalChar.Slash : ReportUiModels.DecimalChar.Dot;
        }

        public Format()
        {

        }

        public bool DigitGroup { get; set; }

        public int? NumberDigits { get; set; }

        public GroupNumberChar? GroupNumberChar { get; set; }

        public DecimalChar? DecimalChar { get; set; }

        public XElement GetXml()
        {
            NumberDigits = NumberDigits.GetValueOrDefault(5);
            if (!GroupNumberChar.HasValue)
                GroupNumberChar = ReportUiModels.GroupNumberChar.Camma;
            if (!DecimalChar.HasValue)
                DecimalChar = ReportUiModels.DecimalChar.Dot;
            return new XElement("StringFormat").AddAttribute("DigitGroup", DigitGroup).AddAttribute("NumberDigits", NumberDigits)
                .AddAttribute("GroupNumberChar", GroupNumberChar).AddAttribute("DecimalChar", DecimalChar);
        }

        public string GetJson()
        {
            var str = "{digitGroup:" + DigitGroup.ToString().ToLower();
            if (NumberDigits.HasValue)
                str += ",numberDigits:" + NumberDigits.Value;
            if (GroupNumberChar.HasValue)
                str += ",groupNumberChar:" + GroupNumberChar.ConvertToInt();
            if (DecimalChar.HasValue)
                str += ",DecimalChar:" + DecimalChar.ConvertToInt();
            str += "}";
            return str;
        }

        public string GetFormatString(string member)
        {
            if (!member.HasValue())
                return null;
            if (!DigitGroup && NumberDigits.GetValueOrDefault(5) == 5)
                return null;
            if (member[0] == '{')
                member = member.Substring(1);
            if (member[member.Length - 1] == '}')
                member = member.Substring(0, member.Length - 1);
            var formatStr = "0:";
            if (DigitGroup)
                formatStr += "#,";
            formatStr += "0.";
            for (int i = 0; i < NumberDigits.GetValueOrDefault(5); i++)
                formatStr += '#';
            member = "string.Format(\"{" + formatStr + "}\", " + member + ')';
            if (DecimalChar == ReportUiModels.DecimalChar.Slash)
                member += ".Replace('.', '/')";
            if (GroupNumberChar == ReportUiModels.GroupNumberChar.Dot)
                member += ".Replace(',', '.')";
            return '{' + member + '}';
        }
    }
}