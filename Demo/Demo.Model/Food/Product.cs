using Caspian.Common;
using Caspian.Engine;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Demo.Model
{
    [Table("Products", Schema = "demo")]
    public class Product
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// عنوان محصول
        /// </summary>
        [DisplayName("عنوان"), ReportField("عنوان محصول")]
        public string Title { get; set; }

        /// <summary>
        /// کد
        /// </summary>
        [DisplayName("کد")]
        public string Code { get; set; }

        /// <summary>
        /// کد گروه محصول
        /// </summary>
        [DisplayName("گروه محصول")]
        public int ProductCategoryId { get; set; }

        /// <summary>
        /// قیمت
        /// </summary>
        [DisplayName("قیمت"), ReportField]
        public int Price { get; set; }

        /// <summary>
        /// قیمت بیرون بر
        /// </summary>
        [DisplayName("قیمت بیرون بر"), ReportField]
        public int PriceOuterBound { get; set; }

        /// <summary>
        /// وعده های غذایی
        /// </summary>
        [DisplayName("وعده های غذایی")]
        public Meal Meal { get; set; }

        /// <summary>
        /// اتمام فروش
        /// </summary>
        [DisplayName("اتمامی فروش")]
        public bool OutofStock { get; set; }

        /// <summary>
        /// تخفیف پذیر
        /// </summary>
        [DisplayName("تخفیف پذیر")]
        public bool Discountable { get; set; }

        /// <summary>
        /// وضعیت فعال
        /// </summary>
        [DisplayName("وضعیت")]
        public ActiveType ActiveType { get; set; }

        /// <summary>
        /// تصویر
        /// </summary>
        [DisplayName("تصویر")]
        public byte[] Image { get; set; }

        /// <summary>
        /// مشخصات گروه محصول 
        /// </summary>
        [ForeignKey(nameof(ProductCategoryId)), ReportField("گروه محصول")]
        public virtual ProductCategory ProductCategory { get; set; }

        /// <summary>
        /// سفارشهای محصول
        /// </summary>
        [CheckOnDelete("برای محصول سفارش ثبت شده و امکان حذف آن وجود ندارد")]
        public virtual IList<OrderDeatil> OrderDeatils { get; set; }
    }
}
