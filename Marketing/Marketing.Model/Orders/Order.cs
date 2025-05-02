using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Marketing.Model
{
    [Table("Orders", Schema = "mrk")]
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("تاریخ شفارش")]
        public DateOnly OrderDate { get; set; }

        [DisplayName("مشتری")]
        public int? CustomerId { get; set; }

        [ForeignKey(nameof(CustomerId))]
        public Customer Customer { get; set; }

        [DisplayName("نوع پرداخت")]
        public SettleType? SettleType { get; set; }

        [DisplayName("شماره سفارش")]
        public int OrderNumber { get; set; }

        [DisplayName("آدرس مشتری")]
        public int? AddressId { get; set; }

        [ForeignKey(nameof(AddressId))]
        public CustomerAddress Address { get; set; }

        [CheckOnDelete("سفارش دارای اقلام سفارشی هست و امکان حذف آن وجود ندارد.")]
        public ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
