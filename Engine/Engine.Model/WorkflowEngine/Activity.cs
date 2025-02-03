using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Engine
{
    [Table("Activities", Schema = "cmn")]
    public class Activity
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// عنوان
        /// </summary>
        [DisplayName("عنوان")]
        public string Title { get; set; }

        [DisplayName("نام")]
        public string Name { get; set; }

        [DisplayName("شرح")]
        public string Description { get; set; }

        public ActivityType ActivityType { get; set; }

        public TaskType? TaskType { get; set; }

        public string SourceCodeFileName { get; set; }

        public int? WorkflowFormId { get; set; }

        [ForeignKey(nameof(WorkflowFormId))]
        public WorkflowForm WorkflowForm { get; set; }

        public double Left { get; set; }

        public double Top { get; set; }

        public GatewayType? GatewayType { get; set; }

        public EventTriggerType? EventTriggerType { get; set; }

        /// <summary>
        /// نوع عامل
        /// </summary>
        [DisplayName("نوع عامل")]
        public ActorType? ActorType { get; set; }

        [DisplayName("گردش کار")]
        public int WorkflowId { get; set; }

        [ForeignKey(nameof(WorkflowId))]
        public Workflow Workflow { get; set; }

        [InverseProperty(nameof(NodeConnector.Activity))]
        public IList<NodeConnector> OutConnectors { get; set; }

        [InverseProperty(nameof(NodeConnector.ToActivity))]
        public IList<NodeConnector> InConnectors { get; set; }

        public ICollection<TaskOperation> TaskOperations { get; set; }
    }
}
