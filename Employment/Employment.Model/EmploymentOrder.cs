using Caspian.Engine;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Employment.Model
{
    [RuleType("اعضاء غیرهیئت علمی"), WorkflowEntity("حکم غیرهیئت علمی")]
    public class EmploymentOrder
    {
        /// <summary>
        /// رتبه
        /// </summary>
        [DisplayName("رتبه"), Rule("رتبه")]
        public int Rank { get; set; }

        /// <summary>
        /// پایه
        /// </summary>
        [DisplayName("پایه"), Rule("پایه")]
        public BaseType BaseType { get; set; }

        /// <summary>
        /// تعداد سال خدمت
        /// </summary>
        [DisplayName("تعداد سال خدمت"), Rule("تعداد سال خدمت")]
        public int YearOfEmployment { get; set; } = 0;

        /// <summary>
        /// پایه تحصیلی
        /// </summary>
        [DisplayName("پایه تحصیلی"), Rule("پایه تحصیلی")]
        public BaseStudy BaseStudy { get; set; }

        /// <summary>
        /// ضریب حقوقی
        /// </summary>
        [DisplayName("ضریب حقوقی"), Rule("ضریب حقوقی")]
        public int SalaryFactor { get; set; }

        /// <summary>
        /// کد نوع حکم
        /// </summary>
        public int EmploymentOrderTypeId { get; set; }

        /// <summary>
        /// مشخصات نوع جکم
        /// </summary>
        [ForeignKey(nameof(EmploymentOrderTypeId))]
        public virtual EmploymentOrderType EmploymentOrderType { get; set; }
    }
}
