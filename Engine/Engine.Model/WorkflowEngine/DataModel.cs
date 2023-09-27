using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Engine
{
    [Table("DataModels", Schema = "cmn")]
    public class DataModel
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("Name")]
        public string Name { get; set; }

        [DisplayName("Title")]
        public string Title { get; set; }

        [DisplayName("Workflow group")]
        public int WorkflowGroupId { get; set; }

        [ForeignKey(nameof(WorkflowGroupId))]
        public virtual WorkflowGroup WorkflowGroup { get; set; }

        [DisplayName("Description")]
        public string Description { get; set; }

        [CheckOnDelete("Data model has Workflow(s) and can not removed")]
        public virtual IList<Workflow> Workflows { get; set; }

        [CheckOnDelete("Data model has field(s) and can not removed")]
        public virtual IList<DataModelField> Fields { get; set; }

        [CheckOnDelete("Data model has forms(s) and can not removed")]
        public virtual IList<WorkflowForm> WorkflowForms { get;set; }
    }
}
