using Caspian.Engine;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Employment.Model
{
    /// <summary>
    /// مشخصات فرزندان
    /// </summary>
    [WorkflowEntity("مشخصات فرزندان")]
    public class ChildrenProperties
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// نام
        /// </summary>
        [DisplayName("نام")]
        public string FName { get; set; }

        /// <summary>
        /// نام خانوادگی
        /// </summary>
        [DisplayName("نام خانوادگی")]
        public string LName { get; set; }

        /// <summary>
        /// تاریخ تولد
        /// </summary>
        [DisplayName("تاریخ تولد")]
        public DateTime BirthDate { get; set; }

        /// <summary>
        /// جنسیت
        /// </summary>
        [DisplayName("جنسیت")]
        public Gender Gender { get; set; }

        /// <summary>
        /// تاریخ ازدواج
        /// </summary>
        [DisplayName("تاریخ ازدواج")]
        public DateTime? MarriageDate { get; set; }

        public byte? BirthOrder { get; set; }

        /// <summary>
        /// کد شهر محل تولد
        /// </summary>
        [DisplayName("شهر محل تولد")]
        public int BirthCityId { get; set; }

        /// <summary>
        /// مشخصات شهر تولد
        /// </summary>
        [ForeignKey(nameof(BirthCityId))]
        public virtual City BirthCity { get; set; }

        /// <summary>
        /// کد کارمند
        /// </summary>
        public int EmployeeId { get; set; }

        /// <summary>
        /// مشخصات کارمند
        /// </summary>
        [ForeignKey(nameof(EmployeeId))]
        public virtual Employee Employee { get; set; }
    }
}
