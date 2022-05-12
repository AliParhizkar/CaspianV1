using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Engine
{
    [Table("HtmlColumns", Schema = "cmn")]
    public class HtmlColumn
    {
        [Key]
        public int Id { get; set; }

        public byte Span { get; set; } = 12;

        public bool Hidden { get; set; }  

        public int? RowId { get; set; }

        [ForeignKey(nameof(RowId))]
        public virtual HtmlRow Row { get; set; }

        public virtual IList<InnerRow> InnerRows { get; set; }
    }

    [Table("InnerRows", Schema = "cmn")]
    public class InnerRow
    {
        public int Id { get; set; }

        public byte Span { get; set; }

        public int? ComponentId { get; set; }

        [ForeignKey(nameof(ComponentId))]
        public virtual Component Component { get; set; }

        public byte ColumnsCount { get; set; }
    }
}
