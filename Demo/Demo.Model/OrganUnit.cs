using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Demo.Model
{
    [Table("OrganUnits", Schema = "demo")]
    public class OrganUnit
    {
        [Key]
        public int Id { get; set; }

        public int? ParentOrganId { get; set; }

        [ForeignKey(nameof(ParentOrganId))]
        public virtual OrganUnit ParentOrgan { get; set; }

        [DisplayName("Title")]
        public string Title { get; set; }

        public ActiveType ActiveType { get; set; }

        [CheckOnDelete("Organ unit has subunit and can not be removed")]
        public virtual ICollection<OrganUnit> SuborganUnits { get; set; }
    }

    [Table("Test", Schema = "demo")]
    public class Test
    {
        [Key]
        public int Id { get; set; }

        public string Title { get; set; }
    }
}
