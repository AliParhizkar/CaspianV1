using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Demo.Model
{
    [Table("Subunits", Schema = "demo")]
    public class Subunit
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("Title")]
        public string Title { get; set; }

        [DisplayName("Main Unit")]
        public int MainUnitId { get; set; }

        [DisplayName("Factor")]
        public int Factor { get; set; }

        [ForeignKey(nameof(MainUnitId))]
        public virtual MainUnit MainUnit { get; set; }

        [DisplayName("Status")]
        public ActiveType ActiveType { get; set; }

        [CheckOnDelete("Sub unit has material")]
        public virtual IList<Material> Materials { get; set; }
    }
}
