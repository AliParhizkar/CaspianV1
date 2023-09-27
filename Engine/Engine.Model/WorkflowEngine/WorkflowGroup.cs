using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Engine
{
    [Table("WorkflowGroups", Schema = "cmn")]
    public class WorkflowGroup
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("Title")]
        public string Title { get; set; }

        [DisplayName("Subsystem")]
        public SubSystemKind SubSystemKind { get; set; }

        [DisplayName("Description")]
        public string Description { get; set; }

        [CheckOnDelete("This group has Data model and can not removed")]
        public virtual IList<DataModel> DataModels { get; set; }

        [CheckOnDelete("This group has Workflow and can not removed")]
        public virtual IList<Workflow> Workflows { get; set; }
    }
}
