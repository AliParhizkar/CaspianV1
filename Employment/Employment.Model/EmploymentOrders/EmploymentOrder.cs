using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Employment.Model
{
    [Table("EmploymentsOrders", Schema = "emp")]
    public class EmploymentOrder
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// کد واحد سازمانی
        /// </summary>
        [DisplayName("واحد سازمانی")]
        public int OrganUnitId { get; set; }

        /// <summary>
        /// واحد سازمانی
        /// </summary>
        [ForeignKey(nameof(OrganUnitId))]
        public virtual OrganUnit OrganUnit { get; set; }

        /// <summary>
        /// کد پست سازمانی
        /// </summary>
        [DisplayName("پست سازمانی")]
        public int? OrganPostId { get; set; }

        /// <summary>
        /// کد پست سازمانی
        /// </summary>
        [ForeignKey(nameof(OrganPostId))]
        public virtual OrganPost OrganPost { get; set; }

        /// <summary>
        /// تاریخ اجرا
        /// </summary>
        [DisplayName("تاریخ اجرا")]
        public DateTime RunDate { get; set; }

        /// <summary>
        /// تاریخ ثبت
        /// </summary>
        [DisplayName("تاریخ ثبت")]
        public DateTime RegisterDate { get; set; }

        /// <summary>
        /// کد رشته تحصیلی
        /// </summary>
        [DisplayName("رشته تحصیلی")]
        public int MajorId { get; set; }

        /// <summary>
        /// مشخصات رشته تحصیلی
        /// </summary>
        [ForeignKey(nameof(MajorId))]
        public virtual Major Major { get; set; }

        /// <summary>
        /// رتبه
        /// </summary>
        [DisplayName("رتبه")]
        public BaseType BaseType { get; set; }

        /// <summary>
        /// پایه
        /// </summary>
        [DisplayName("پایه")]
        public byte Rank { get; set; }

        [DisplayName("شرح")]
        public string Descript { get; set; }

        public int EmploymentOrderTypeId { get; set; }

        [ForeignKey(nameof(EmploymentOrderTypeId))]
        public virtual EmploymentOrderType EmploymentOrderType { get; set; }
    }
}
