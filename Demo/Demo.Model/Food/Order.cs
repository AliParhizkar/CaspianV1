using Engine.Model;
using Caspian.Engine;
using Caspian.Common;
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

        /// <summary>
        /// تاریخ سفارش
        /// </summary>
        [DisplayName("تاریخ سفارش"), ReportField("تاریخ سفارش")]
        public DateTime? Date { get; set; }

        //[ForeignKey(nameof(Date))]
        //public PersianDateTable PersianDate { get; set; }

        /// <summary>
        /// کد مشتری
        /// </summary>
        [DisplayName("مشتری")]
        public int? CustomerId { get; set; }

        [DisplayName("شماره سفارش")]
        public int? OrderNo { get; set; }

        /// <summary>
        /// مشخصات مشتری
        /// </summary>
        [ForeignKey(nameof(CustomerId)),ReportField("مشخصات مشتری")]
        public virtual Customer Customer { get; set; }

        public int? DeliveryId { get; set; }

        [ForeignKey(nameof(DeliveryId))]
        public virtual Delivery Delivery { get; set; }

        /// <summary>
        /// نوع سفارش
        /// </summary>
        [DisplayName("نوع سفارش"), ReportField("نوع سفارش")]
        public OrderType OrderType { get; set; }

        /// <summary>
        /// وضعیت سفارش
        /// </summary>
        [DisplayName("وضعیت سفارش")]
        public OrderStatus? OrderStatus { get; set; }

        public int? TotalAmount { get; set; }

        /// <summary>
        /// توضیحات
        /// </summary>
        [DisplayName("توضیحات")]
        public string Description { get; set; }

        /// <summary>
        /// محصولات سفارش
        /// </summary>
        [CheckOnDelete("سفارش دارای محصول می باشد و امکان حذف آن وجود ندارد")]
        public virtual IList<OrderDeatil> OrderDeatils { get; set; }
    }
}
