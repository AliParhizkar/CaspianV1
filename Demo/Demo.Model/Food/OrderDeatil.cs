using Caspian.Engine;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Demo.Model
{
    [Table("OrderDeatil", Schema = "demo")]
    public class OrderDeatil
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// کد سفارش
        /// </summary>
        [DisplayName("سفارش")]
        public int OrderId { get; set; }

        /// <summary>
        /// مشخصات سفارش
        /// </summary>
        [ForeignKey(nameof(OrderId)), ReportField("مشخصات سفارش")]
        public virtual Order Order { get; set; }

        [DisplayName("شرح")]
        public string Descript { get; set; }

        /// <summary>
        /// کد محصول
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// مشخصات محصول
        /// </summary>
        [ForeignKey(nameof(ProductId)), ReportField("مشخصات محصول")]
        public virtual Product Product { get; set; }

        /// <summary>
        /// قیمت محصول
        /// </summary>
        [DisplayName("قیمت"), ReportField("قیمت")]
        public int Price { get; set; }

        /// <summary>
        /// تعداد محصول
        /// </summary>
        [DisplayName("تعداد"), ReportField("تعداد")]
        public int Count { get; set; }
    }
}
