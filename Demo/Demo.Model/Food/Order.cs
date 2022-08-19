using System;
using Caspian.Engine;
using Caspian.Common;
using System.ComponentModel;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Demo.Model
{
    [Table("*Orders")]
    public class Order
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// تاریخ سفارش
        /// </summary>
        [DisplayName("تاریخ سفارش"), ReportField("تاریخ سفارش")]
        public DateTime? Date { get; set; }

        /// <summary>
        /// کد مشتری
        /// </summary>
        [DisplayName("مشخصات مشتری")]
        public int? CustomerId { get; set; }

        /// <summary>
        /// مشخصات مشتری
        /// </summary>
        [ForeignKey(nameof(CustomerId)),ReportField("مشخصات مشتری")]
        public virtual Customer Customer { get; set; }

        public int? DynamicParameterId { get; set; }

        [ForeignKey(nameof(DynamicParameterId))]
        public virtual DynamicParameter Parameter { get; set; }

        /// <summary>
        /// نوع سفارش
        /// </summary>
        [DisplayName("نوع سفارش"), ReportField("نوع سفارش")]
        public OrderType OrderType { get; set; }

        /// <summary>
        /// شماره سفارش
        /// </summary>
        [DisplayName("شماره سفارش"), ReportField]
        public int OrderNumber { get; set; }

        public int? TotalAmount { get; set; }

        /// <summary>
        /// محصولات سفارش
        /// </summary>
        [CheckOnDelete("سفارش دارای محصول می باشد و امکان حذف آن وجود ندارد")]
        public virtual IList<OrderDeatil> OrderDeatils { get; set; }
    }
}
