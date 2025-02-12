namespace Caspian.Common
{
    public class CaspianDataService
    {
        public int UserId { get; set; }

        public Language? Language { get; set; }
    }

    public class PageData
    {
        public int UserId { get; set; }

        public Language? Language { get; set; }

        public bool RightToLeft { get; set; }
    }
}
