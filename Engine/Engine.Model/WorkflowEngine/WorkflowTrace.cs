using Caspian.Common;
using System.ComponentModel;
using Caspian.Common.Security;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Engine
{
    /// <summary>
    /// ردیابی مراحل گردش کار
    /// </summary>
    [Table("WorkflowTraces", Schema = "cmn")]
    public class WorkflowTrace
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        /// <summary>
        /// شماره ی ردیابی
        /// </summary>
        [DisplayName("شماره ی ردیابی")]
        public int TraceId { get; set; }

        public int ActivityId { get; set; }

        [ForeignKey(nameof(ActivityId))]
        public virtual Activity Activity { get; set; }

        public int? StartActivityId { get; set; }

        [ForeignKey(nameof(StartActivityId))]
        public virtual Activity StartActivity { get; set; }

        public int? CustomUserId { get; set; }

        [ForeignKey(nameof(CustomUserId))]
        public virtual CustomUser CustomUser { get; set; }  
    }
}
