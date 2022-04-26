using Caspian.Common.Attributes;

namespace Caspian.Common
{
    /// <summary>
    /// نوع مرتب سازی
    /// </summary>
    public enum OrderType : byte
    {
        /// <summary>
        /// صعودی
        /// </summary>
        [EnumField("صعودی")]
        Asc = 1,

        /// <summary>
        /// نزولی
        /// </summary>
        [EnumField("نزولی")]
        Decs = 2
    }
}
