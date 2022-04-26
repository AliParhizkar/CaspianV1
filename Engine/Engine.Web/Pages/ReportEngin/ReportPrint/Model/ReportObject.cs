using System.Xml.Linq;
using System.Threading.Tasks;

namespace ReportUiModels
{
    public class ReportObject
    {
        private static int Ref;

        public static void ResetRef()
        {
            ReportObject.Ref = 0;
        }

        public ReportObject()
        {
            Ref++;
            Id = Ref;
        }

        public int Id { get; private set; }

        public virtual XElement GetXmlElement(ReportType reportType)
        {
            return null;
        }

        public virtual string GetJson()
        {
            return null;
        }
    }
}
