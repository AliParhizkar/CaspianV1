using Caspian.Engine;
using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Demo.Model
{
    [Table("ProductCategories", Schema = "demo")]
    public class ProductCategory
    {
        [Key]
        public int Id { get; set; }

        [ReportField("عنوان گروه محصول"), DisplayName("عنوان")]
        public string Title { get; set; }

        /// <summary>
        /// اولویت
        /// </summary>
        [DisplayName("اولویت")]
        public int Priority { get; set; }

        /// <summary>
        /// کد
        /// </summary>
        [DisplayName("کد")]
        public string Code { get; set; }

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
