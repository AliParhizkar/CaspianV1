using Caspian.Engine;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Employment.Model
{
    /// <summary>
    /// سوابق تحصیلی
    /// </summary>
    [WorkflowEntity("سوابق تحصیلی")]
    public class EducationRecords
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// کد کارمند
        /// </summary>
        public int EmployeeId { get; set; }

        /// <summary>
        /// مشخصات کارمند
        /// </summary>
        [ForeignKey(nameof(EmployeeId))]
        public virtual Employee Employee { get; set; }

        /// <summary>
        /// کد مدرک تحصیلی
        /// </summary>
        public int EducationDegreeId { get; set; }

        /// <summary>
        /// مشخصات مدرک تحصیلی
        /// </summary>
        [ForeignKey(nameof (EducationDegreeId))]
        public virtual EducationDegree EducationDegree { get; set; }

        /// <summary>
        /// کد رشته تحصیلی
        /// </summary>
        public int? MajorId { get; set; }

        /// <summary>
        /// مشخصات رشته تحصیلی
        /// </summary>
        [ForeignKey(nameof(MajorId))]
        public virtual Major Major { get; set; }

        /// <summary>
        /// محل تحصیل
        /// </summary>
        [DisplayName("محل تحصیل")]
        public string Name { get; set; }

        /// <summary>
        /// آدرس
        /// </summary>
        [DisplayName("آدرس")]
        public string Address { get; set; }
    }
}
