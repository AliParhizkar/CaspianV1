using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Demo.Model
{
    [Table("Materials", Schema = "demo")]
    public class Material
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("Title")]
        public string Title { get; set; }

        [DisplayName("Main unit")]
        public int MainUnitId { get; set; }

        [ForeignKey(nameof(MainUnitId))]
        public virtual MainUnit MainUnit { get; set; }

        [DisplayName("Sub unit")]
        public int? SubunitId { get; set; }

        [ForeignKey(nameof(SubunitId))]
        public virtual Subunit Subunit { get; set; }

        [DisplayName("Status")]
        public ActiveType ActiveType { get; set; }

        [CheckOnDelete("The material has receipt and can't be removed")]
        public virtual IList<ReceiptDetail> ReceiptDetails { get; set; }
    }
}
