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

        [DisplayName("نام")]
        public string Name { get; set; }

        [DisplayName("عنوان")]
        public string Title { get; set; }

        [DisplayName("گروه گردش کار")]
        public int WorkflowGroupId { get; set; }

        [ForeignKey(nameof(WorkflowGroupId))]
        public virtual WorkflowGroup WorkflowGroup { get; set; }

        [DisplayName("شرح")]
        public string? Description { get; set; }

        [CheckOnDelete("مدل داده ای دارای گردش کار می باشد و امکان حذف آن وجود ندارد")]
        public virtual IList<Workflow> Workflows { get; set; }

        [CheckOnDelete("نوع داده ای دارای فیلد می باشد و امکان حذف آن وجود ندارد")]
        public virtual IList<DataModelField> Fields { get; set; }
    }
}
