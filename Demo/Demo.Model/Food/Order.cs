using Caspian.Engine;
using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Metrics;

namespace Demo.Model
{
    [Table("Orders", Schema = "demo")]
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("Order date"), ReportField("تاریخ سفارش")]
        public DateTime? Date { get; set; }

        [DisplayName("Customer")]
        public int? CustomerId { get; set; }

        [DisplayName("Order number")]
        public int? OrderNo { get; set; }

        [ForeignKey(nameof(CustomerId)),ReportField("مشخصات مشتری")]
        public virtual Customer Customer { get; set; }

        public int? CourierId { get; set; }

        [ForeignKey(nameof(CourierId))]
        public virtual Courier Courier { get; set; }

        [DisplayName("Order type"), ReportField("نوع سفارش")]
        public OrderType OrderType { get; set; }

        [DisplayName("Status")]
        public OrderStatus? OrderStatus { get; set; }

        public int? TotalAmount { get; set; }

        [DisplayName("Description")]
        public string Description { get; set; }

        [CheckOnDelete("The order has details and can not remove")]
        public virtual IList<OrderDeatil> OrderDeatils { get; set; }
    }
}
