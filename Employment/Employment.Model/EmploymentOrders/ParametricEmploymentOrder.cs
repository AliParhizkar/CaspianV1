using Caspian.Common;
using Caspian.Engine;
using Employment.Model.EmploymentOrders;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Employment.Model
{
    [RuleType("اعضاء غیرهیئت علمی"), WorkflowEntity("حکم غیرهیات علمی"), DynamicType("حکم غیرهیات علمی", typeof(EmploymentOrderDynamicParameterValue))]
    public class ParametricEmploymentOrder
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// پایه
        /// </summary>
        [DisplayName("پایه"), Rule("پایه"), ReportField]
        public int Rank { get; set; }

        /// <summary>
        /// رتبه
        /// </summary>
        [DisplayName("رتبه"), ReportField]
        public BaseType BaseType { get; set; }

        /// <summary>
        /// تعداد سال خدمت
        /// </summary>
        [DisplayName("تعداد سال خدمت")]
        public int YearOfEmployment { get; set; } = 0;

        /// <summary>
        /// ضریب حقوقی
        /// </summary>
        [DisplayName("ضریب حقوقی"), ReportField]
        public int SalaryFactor { get; set; }

        [DisplayName("پایه تحصیلی"), Rule("پایه تحصیلی")]
        public BaseStudy BaseStudy { get; set; }

        [DisplayName("سابقه خدمت"), Rule("سابقه خدمت")]
        public byte? ServiceHistory { get; set; }

        /// <summary>
        /// کد نوع حکم
        /// </summary>
        [DisplayName("نوع جکم")]
        public int EmploymentOrderTypeId { get; set; }

        [Rule("سال اجرای حکم"), DisplayName("سال اجرای حکم")]
        public int Year { get; set; }

        /// <summary>
        /// مشخصات نوع جکم
        /// </summary>
        [ForeignKey(nameof(EmploymentOrderTypeId)), ReportField("نوع حکم")]
        public virtual EmploymentOrderType EmploymentOrderType { get; set; }

        [DisplayName("شرح حکم")]
        public string Descript { get; set; }

        [DisplayName("مدرک تحصیلی")]
        public int MajorId { get; set; }

        [ForeignKey(nameof(MajorId)), ReportField("رشته تحصیلی")]
        public virtual Major Major { get; set; }

        /// <summary>
        /// پارامترهای پویای حکم
        /// </summary>
        [CheckOnDelete("حکم کارگزینی دارای پارامتر پویا می باشد و امکان حذف آن وجود ندارد")]
        public virtual IList<EmploymentOrderDynamicParameterValue> Values { get; set; }
    }
}
