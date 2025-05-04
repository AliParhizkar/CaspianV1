using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Marketing.Model
{
    [Table("OrderDetails", Schema = "mrk")]
    public class OrderDetail
    {
        [Key]
        public int Id { get; set; }

        public int OrderId { get; set; }

        [ForeignKey(nameof(OrderId))]
        public Order Order { get; set; }

        public int ProductId { get; set; }

        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; }

        [DisplayName("تعداد/مقدار")]
        public decimal Quantity { get; set; }

        //[DisplayName("تاپینگ")]
        //public decimal ToppingAmount { get; set; }

        [DisplayName("مبلغ")]
        public int Price { get; set; }

        [DisplayName("تخفیف")]
        public int Discount { get; set; }

        [ComputedSqlColumn("([Price] - [Discount]) * [Quantity]"), DisplayName("قیمت کل")]
        public decimal PriceTotal { get;}

        [MaxLength(255)]
        public string Description { get; set; }

        [CheckOnDelete("این آیتم سفارش دارای تاپینگ است و نمی توان آن را حذف کرد")]
        public IList<OrderDetailTopping> OrderDetailToppings { get; set; }
    }
}
