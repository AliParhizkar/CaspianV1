using Caspian.Common;
using System.Xml.Linq;

namespace Caspian.Server
{
    public class Border: Common.Border
    {
        public Border(XElement element) 
        {
            var array = element.Value.Split(';');
            var borderKindArray = array[0].Split(',');
            var borderKindValue = 0;
            for (var i = 0; i < 4; i++)
                if (borderKindArray.Length > i)
                    borderKindValue |= (int)typeof(BorderKind).GetField(borderKindArray[i]).GetValue(null);
            this.BorderKind = (BorderKind)borderKindValue;
            Color = new Color(array[1]);
            Width = Convert.ToDouble(array[2]);
            var filed = typeof(BorderStyle).GetField(array[3]);
            if (filed != null)
                Style = (BorderStyle)filed.GetValue(null);
        }
    }
}
