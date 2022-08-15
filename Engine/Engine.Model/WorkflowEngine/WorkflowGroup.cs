using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Engine
{
    [Table("WorkflowGroups", Schema = "cmn")]
    public class WorkflowGroup
    {
        public WorkflowGroup()
        {

        }

        [Key]
        public int Id { get; set; }

        [DisplayName("عنوان")]
        public string Title { get; set; }

        [DisplayName("زیرسیستم")]
        public SubSystemKind SubSystemKind { get; set; }

        [DisplayName("توضیحات")]
        public string? Description { get; set; }

        [CheckOnDelete("گروه دارای مدل داده ای می باشد و امکان حذف آن وجود ندارد")]
        public virtual IList<DataModel> DataModels { get; set; }

        [CheckOnDelete("گروه دارای گردش کار می باشد و امکان حذف آن وجود ندارد")]
        public virtual IList<Workflow> Workflows { get; set; }

        [CheckOnDelete("گروه دارای فرم می باشد و امکان حذف آن وجود ندارد")]
        public virtual IList<WorkflowForm> WorkflowGroups { get; set; }
    }
}
