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

        /// <summary>
        /// فرمهایی که به این فعالیت تخصیص داده شده اند
        /// </summary>
        public virtual IList<ActivityFormAssign> FormAssigns { get; set; }

        //public virtual IList<WorkflowTrace> WorkflowTraces { get; set; }    
    }
}
