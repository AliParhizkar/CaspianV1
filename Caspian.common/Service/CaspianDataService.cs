using Microsoft.Extensions.DependencyInjection;

namespace Caspian.Common
{
    public class CaspianDataService
    {
        public int UserId { get; set; }

        public Language? Language { get; set; }
    }

    public class PageData
    {
        public int? PageId { get; set; }

        public int UserId { get; set; }

        public Language? Language { get; set; }

        public bool RightToLeft { get; set; }

        public Action<string, string> OnClick { get; set; }
    }
}
