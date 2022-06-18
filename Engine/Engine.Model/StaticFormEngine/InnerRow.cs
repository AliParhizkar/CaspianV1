using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Engine
{
    [Table("InnerRows", Schema = "cmn")]
    public class InnerRow
    {
        [Key]
        public int Id { get; set; }

        public byte Span { get; set; }

        public int HtmlColumnId { get; set; }

        [ForeignKey(nameof(HtmlColumnId))]
        public virtual HtmlColumn HtmlColumn { get; set; }

        public virtual IList<HtmlColumn> HtmlColumns { get; set; }
    }
}
