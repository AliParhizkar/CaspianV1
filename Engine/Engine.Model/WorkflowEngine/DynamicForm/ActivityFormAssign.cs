using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Engine
{
    [Table("ActivitiesFormsAssign", Schema = "cmn")]
    public class ActivityFormAssign
    {
        [Key]
        public int Id { get; set; }

        public int WorkflowFormId { get; set; }

        [ForeignKey(nameof(WorkflowFormId))]
        public virtual WorkflowForm WorkflowForm { get; set; }

        public int ActivityId { get; set; }

        [ForeignKey(nameof(ActivityId))]
        public virtual Activity Activity { get; set; }
    }
}
