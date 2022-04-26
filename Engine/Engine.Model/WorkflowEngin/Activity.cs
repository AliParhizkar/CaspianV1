using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Engine
{
    [Table("Activities", Schema = "cmn")]
    public class Activity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        /// <summary>
        /// عنوان
        /// </summary>
        [DisplayName("عنوان")]
        public string Title { get; set; }

        public CategoryType CategoryType { get; set; }

        public int Left { get; set; }

        public int Top { get; set; }

        /// <summary>
        /// نوع عامل
        /// </summary>
        [DisplayName("نوع عامل")]
        public ActorType? ActorType { get; set; }

        public int? ActionId { get; set; }

        [ForeignKey(nameof(ActionId))]
        public virtual Action Action { get; set; }

        [DisplayName("گردش کار")]
        public int WorkflowId { get; set; }

        [ForeignKey(nameof(WorkflowId))]
        public virtual Workflow Workflow { get; set; }

        /// <summary>
        /// مشخصات فیلدهای استاتیک و نحوهی نمایش آنها
        /// </summary>
        public virtual IList<ActivityField> Fields { get; set; }

        [InverseProperty(nameof(Activity))]
        public virtual IList<Connector> OutConnectors { get; set; }

        [InverseProperty("ToActivity")]
        public virtual IList<Connector> InConnectors { get; set; }

        /// <summary>
        /// مشخصات فیلدهای پویا و زمان نوع نمایش آنها و کد کنترل مربوط به آنها
        /// </summary>
        public virtual IList<ActivityDynamicField> DynamicFields { get; set; }
    }
}
