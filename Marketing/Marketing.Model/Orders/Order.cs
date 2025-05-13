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

        [DisplayName("زمان سفارش")]
        public TimeOnly OrderTime { get; set; }

        [DisplayName("مشتری")]
        public int? CustomerId { get; set; }

        [ForeignKey(nameof(CustomerId))]
        public Customer Customer { get; set; }

        [DisplayName("نوع پرداخت")]
        public SettleType? SettleType { get; set; }

        [DisplayName("نوع سفارش")]
        public OrderType OrderType { get; set; }

        [DisplayName("پرداخت شده")]
        public bool IsSettled { get; set; }

        [DisplayName("مبلغ نقد")]
        public decimal CashAmount { get; set; }

        [DisplayName("مبلغ کارت")]
        public decimal CardAmount {  get; set; }

        [DisplayName("مبلغ حساب")]
        public decimal AccountingAmount { get; set; }

        [DisplayName("شماره سفارش")]
        public int OrderNumber { get; set; }

        [DisplayName("نوع تخفیف")]
        public DiscountType DiscountType { get; set; }

        [DisplayName("درصد تخفیف")]
        public decimal PercentDiscount { get; set; }

        [DisplayName("مبلغ تخفیف")]
        public decimal DiscountAmount { get; set; }

        [DisplayName("قیمت محصولات")]
        public decimal ProductAmount { get; set; }

        [DisplayName("مبلغ قابل پرداخت")]
        public decimal PaymentAmount { get; set; }

        [DisplayName("روش رند کردن")]
        public RoundType? RoundType { get; set; }

        [DisplayName("مبلغ رند")]
        public decimal? RoundAmount { get; set; }

        [DisplayName("آدرس مشتری")]
        public int? AddressId { get; set; }

        [ForeignKey(nameof(AddressId))]
        public CustomerAddress Address { get; set; }

        [CheckOnDelete("سفارش دارای اقلام سفارشی هست و امکان حذف آن وجود ندارد.")]
        public ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
