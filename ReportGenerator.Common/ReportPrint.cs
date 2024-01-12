
namespace Caspian.Common
{
    public class ReportPrint 
    {
        public ReportPrint()
        {
            Pages = new List<ReportPrintPage>();
        }

        public IEnumerable<ReportPrintPage> Pages { get; set; }

        public string GetJson()
        {
            throw new NotImplementedException();   
        }
    }
}