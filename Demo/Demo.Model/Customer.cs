using Caspian.Common;
using Caspian.Engine;
using System.ComponentModel;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Demo.Model
{
    [Table("*Customers")]
    public class Customer
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// نوع مشتری
        /// </summary>
        [DisplayName("نوع مشتری"), ReportField]
        public CustomerType CustomerType { get; set; }

        /// <summary>
        /// نام
        /// </summary>
        [DisplayName("نام"), ReportField]
        public string FName { get; set; }

        /// <summary>
        /// نام خانوادگی
        /// </summary>
        [DisplayName("نام خانوادگی"), ReportField]
        public string LName { get; set; }

        /// <summary>
        /// جنسیت
        /// </summary>
        [DisplayName("جنسیت"), ReportField]
        public Gender? Gender { get; set; }

        /// <summary>
        /// عنوان شرکت
        /// </summary>
        [DisplayName("عنوان شرکت"), ReportField]
        public string CompanyName { get; set; }

        /// <summary>
        /// شماره مشتری
        /// </summary>
        [DisplayName("شماره مشتری")]
        public int CustomerNumber { get; set; }

        /// <summary>
        /// شماره همراه
        /// </summary>
        [DisplayName("شماره همراه"), ReportField]
        public string MobileNumber { get; set; }

        /// <summary>
        /// تلفن
        /// </summary>
        [DisplayName("تلفن"), ReportField]
        public string Tel { get; set; }

        /// <summary>
        /// مشخصات سفارشهای مشتری
        /// </summary>
        [CheckOnDelete("مشتری دارای سفارش می باشد")]
        public virtual IList<Order> Orders { get; set; }

        /// <summary>
        /// عضویت های مشتری در گروه ها مشتریان
        /// </summary>
        [CheckOnDelete("مشتری عضو گروه می باشد و امکان حذف وی وجود ندارد.")]
        public virtual IList<CustomerGroupMembership> CustomerGroupMemberships { get; set; }
    }
}
