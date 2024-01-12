namespace Caspian.Common
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

        public virtual string GetJson()
        {
            return null;
        }
    }
}
