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

        [DisplayName("Order")]
        public int OrderId { get; set; }

        /// <summary>
        /// مشخصات سفارش
        /// </summary>
        [ForeignKey(nameof(OrderId)), ReportField("مشخصات سفارش")]
        public virtual Order Order { get; set; }

        [DisplayName("Descript")]
        public string Descript { get; set; }

        public int ProductId { get; set; }

        [ForeignKey(nameof(ProductId)), ReportField("مشخصات محصول")]
        public virtual Product Product { get; set; }

        [DisplayName("Price"), ReportField("قیمت")]
        public int Price { get; set; }

        [DisplayName("Count"), ReportField("تعداد")]
        public int Count { get; set; }
    }
}
