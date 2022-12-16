using Caspian.Engine;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Employment.Model
{
    /// <summary>
    /// سوابق سازمانی
    /// </summary>
    [WorkflowEntity("سوابق سازمانی")]
    public class WorkHistory
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
    }
}
