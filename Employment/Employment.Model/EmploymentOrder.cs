using Caspian.Engine;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Employment.Model
{
    [RuleType("اعضاء غیرهیئت علمی"), WorkflowEntity("حکم غیرهیات علمی"), DynamicTypeAttribute("حکم غیرهیات علمی", typeof(EmploymentOrderDynamicParameterValue))]
    public class EmploymentOrder
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// رتبه
        /// </summary>
        [DisplayName("رتبه"), Rule("رتبه")]
        public int Rank { get; set; }

        /// <summary>
        /// پایه
        /// </summary>
        [DisplayName("پایه")]
        public BaseType BaseType { get; set; }

        /// <summary>
        /// تعداد سال خدمت
        /// </summary>
        [DisplayName("تعداد سال خدمت")]
        public int YearOfEmployment { get; set; } = 0;

        /// <summary>
        /// ضریب حقوقی
        /// </summary>
        [DisplayName("ضریب حقوقی")]
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
        [ForeignKey(nameof(EmploymentOrderTypeId))]
        public virtual EmploymentOrderType EmploymentOrderType { get; set; }

        [DisplayName("شرح حکم")]
        public string Descript { get; set; }

        [DisplayName("مدرک تحصیلی")]
        public int EducationDegreeId { get; set; }

        [ForeignKey(nameof(EducationDegreeId))]
        public virtual EducationDegree EducationDegree { get; set; }
    }
}
