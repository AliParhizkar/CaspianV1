using System.ComponentModel.DataAnnotations;

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
        [Display(Name = "صعودی")]
        Asc = 1,

        /// <summary>
        /// نزولی
        /// </summary>
        [Display(Name = "نزولی")]
        Decs = 2
    }
}
