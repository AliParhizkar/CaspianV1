using Caspian.Engine;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Employment.Model
{
    [RuleType("اعضاء غیرهیئت علمی"), WorkflowEntity("حکم غیرهیئت علمی")]
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
        [DisplayName("پایه"), Rule("پایه")]
        public BaseType BaseType { get; set; }

        /// <summary>
        /// تعداد سال خدمت
        /// </summary>
        [DisplayName("تعداد سال خدمت"), Rule("تعداد سال خدمت")]
        public int YearOfEmployment { get; set; } = 0;

        /// <summary>
        /// ضریب حقوقی
        /// </summary>
        [DisplayName("ضریب حقوقی"), Rule("ضریب حقوقی")]
        public int SalaryFactor { get; set; }

        /// <summary>
        /// کد نوع حکم
        /// </summary>
        [DisplayName("نوع جکم")]
        public int EmploymentOrderTypeId { get; set; }

        /// <summary>
        /// مشخصات نوع جکم
        /// </summary>
        [ForeignKey(nameof(EmploymentOrderTypeId))]
        public virtual EmploymentOrderType EmploymentOrderType { get; set; }

        [DisplayName("شرح حکم")]
        public string Descript { get; set; }

        [DisplayName("مدرک تحصیلی")]
        public int EducationDegreeId { get; set; }

        [DisplayName("ضریب مدرک تحصیلی")]
        public int? EducationDegreeFactor { get; set; }

        [ForeignKey(nameof(EducationDegreeId))]
        public virtual EducationDegree EducationDegree { get; set; }
    }
}
