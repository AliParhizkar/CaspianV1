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

        public SubSystemKind SubSystemKind { get; set; }

        public byte ColumnCount { get; set; }

        public string? Name { get; set; }

        public string? SourceFileName { get; set; }

        /// <summary>
        /// Rows of form
        /// </summary>
        public virtual IList<HtmlRow> Rows { get; set; }

        /// <summary>
        /// Entity fields that declare in form
        /// </summary>
        public virtual IList<WfFormEntityField> EntityFields { get; set; }

        /// <summary>
        /// All activities that form assigned them
        /// </summary>
        public virtual IList<ActivityFormAssign> Activities { get; set; }
    }
}
