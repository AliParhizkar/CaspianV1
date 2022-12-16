using Caspian.Engine;
using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Demo.Model
{
    /// <summary>
    /// گروه محصول مالی
    /// </summary>
    public class FinancialProductCategory
    {
        [Key]
        public int Id { get; set; }

        [ReportField("عنوان گروه محصول"), DisplayName("عنوان")]
        public string Title { get; set; }

        /// <summary>
        /// وضعیت فعال
        /// </summary>
        [DisplayName("وضعیت")]
        public ActiveType ActiveType { get; set; }

        /// <summary>
        /// لیست محصولات گروه محصول
        /// </summary>
        [CheckOnDelete("برای گروه محصول محصول تعریف شده است و امکان حذف آن وجود ندارد")]
        public virtual IList<Product> Products { get; set; }
    }
}
