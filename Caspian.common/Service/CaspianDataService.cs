using System;
namespace Caspian.Common
{
    public class CaspianDataService
    {
        public int UserId { get; set; }

        public Language? Language { get; set; } = Common.Language.Fa;
    }

    public enum Language
    {
        Fa = 1,
        En
    }
}
