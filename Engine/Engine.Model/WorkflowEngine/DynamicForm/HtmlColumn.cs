using Caspian.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Engine
{
    [Table("HtmlColumns", Schema = "cmn")]
    public class HtmlColumn
    {
        [Key]
        public int Id { get; set; }

        public byte? Span { get; set; } = 12;

        public bool Hidden { get; set; }  

        public int? RowId { get; set; }

        [ForeignKey(nameof(RowId))]
        public HtmlRow Row { get; set; }

        public int? InnerRowId { get; set; }   

        [ForeignKey(nameof(InnerRowId))]
        public InnerRow InnerRow { get; set; }

        [CheckOnDelete(false)]
        [InverseProperty("HtmlColumn")]
        public IList<InnerRow> InnerRows { get; set; }

        public BlazorControl Component { get; set; }
    }
}
