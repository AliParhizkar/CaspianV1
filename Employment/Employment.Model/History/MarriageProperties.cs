using Caspian.Engine;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Employment.Model
{
    /// <summary>
    /// وضعیت تاهل
    /// </summary>
    [WorkflowEntity("وضعیت تاهل")]
    public class MarriageProperties
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
        /// تاریخ ازدواج
        /// </summary>
        [DisplayName("تاریخ ازدواج")]
        public DateTime MarriageDate { get; set; } = DateTime.Now;

        [DisplayName("تاریخ تولد")]
        public DateTime? BirthDate { get; set; } = DateTime.Now;

        /// <summary>
        /// کد مشخصات کارمند
        /// </summary>
        public int EmployeeId { get; set; }

        /// <summary>
        /// مشخصات کارمند
        /// </summary>
        [ForeignKey(nameof(EmployeeId))]
        public virtual Employee Employee { get; set; }
    }
}
