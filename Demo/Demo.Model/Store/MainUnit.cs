using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Demo.Model
{
    [Table("MainUnits", Schema = "demo")]
    public class MainUnit
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("Title")]
        public string Title { get; set; }

        [DisplayName("Status")]
        public ActiveType ActiveType { get; set; }

        /// <summary>
        /// Subunits of unit
        /// </summary>
        [CheckOnDelete("Unit has subunit and can not removed")]
        public virtual IList<Subunit> Subunits { get; set; }

        /// <summary>
        /// Materials of unit
        /// </summary>
        [CheckOnDelete("Unit has materials and can not removed")]
        public virtual IList<Material> Materials { get; set; }
    }
}
