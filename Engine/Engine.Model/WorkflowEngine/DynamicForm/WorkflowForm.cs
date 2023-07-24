using Caspian.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Engine
{
    [Table("WorkflowsForms", Schema = "cmn")]
    public class WorkflowForm
    {
        [Key]
        public int Id { get; set; }

        public string Title { get; set; }

        public int WorkflowGroupId { get; set; }

        [ForeignKey(nameof(WorkflowGroupId))]
        public virtual WorkflowGroup WorkflowGroup { get; set; }

        public int DataModelId { get; set; }

        [ForeignKey(nameof(DataModelId))]
        public virtual DataModel DataModel { get; set; }

        public byte ColumnCount { get; set; }

        public string Name { get; set; }

        public string SourceFileName { get; set; }

        /// <summary>
        /// Rows of form
        /// </summary>
        [CheckOnDelete("فرم دااری ردیف می باشد و امکان حذف آن وجود ندادر")]
        public virtual IList<HtmlRow> Rows { get; set; }
    }
}
