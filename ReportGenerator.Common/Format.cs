
namespace Caspian.Common
{
    public class Format
    {
        public bool DigitGroup { get; set; }

        public int? NumberDigits { get; set; }

        public GroupNumberChar? GroupNumberChar { get; set; }

        public DecimalChar? DecimalChar { get; set; }

        public string GetJson()
        {
            throw new NotImplementedException();
        }

        public string GetFormatString(string member)
        {
            if (member != null)
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
            if (DecimalChar == Common.DecimalChar.Slash)
                member += ".Replace('.', '/')";
            if (GroupNumberChar == Common.GroupNumberChar.Dot)
                member += ".Replace(',', '.')";
            return '{' + member + '}';
        }
    }
}