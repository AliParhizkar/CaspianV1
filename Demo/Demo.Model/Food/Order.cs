using Caspian.Common;
using Caspian.Engine.Model;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Demo.Model
{
    [Table("Orders", Schema = "demo")]
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("Order date")]
        public DateTime Date { get; set; }

        [ForeignKey(nameof(Date))]
        public PersianDateTable PersianDate { get; set; }

        [DisplayName("Customer")]
        public int? CustomerId { get; set; }

        [DisplayName("Order number")]
        public int? OrderNo { get; set; }

        [ForeignKey(nameof(CustomerId))]
        public Customer Customer { get; set; }

        public int? CourierId { get; set; }

        [ForeignKey(nameof(CourierId))]
        public Courier Courier { get; set; }

        [DisplayName("Order type")]
        public OrderType OrderType { get; set; }

        [DisplayName("Status")]
        public OrderStatus? OrderStatus { get; set; }

        public int? TotalAmount { get; set; }

        [DisplayName("Description")]
        public string Description { get; set; }

        [CheckOnDelete("The order has details and can not remove")]
        public IList<OrderDetail> OrderDetails { get; set; }
    }
}
