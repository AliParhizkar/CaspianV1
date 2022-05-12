using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Engine
{
    [Table("HtmlRows", Schema = "cmn")]
    public class HtmlRow
    {
        [Key]
        public int Id { get; set; }

        public byte Span { get; set; }

        public int WorkflowFormId { get; set; }

        [ForeignKey(nameof(WorkflowFormId))]
        public virtual WorkflowForm WorkflowForm { get; set; }

        public virtual IList<HtmlColumn> Columns { get; set; }
    }
}
