using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Investment.Model
{
    [Table("OrganUnits", Schema = "ivm")]
    public class OrganUnit
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("عنوان")]
        public string Title { get; set; }

        [MaxLength(200), DisplayName("شرح")]
        public string Descript { get; set; }

        [DisplayName("وضعیت")]
        public ActiveStatus ActiveStatus { get; set; }

        public int? ParentOrganUnitId { get; set; }

        [ForeignKey(nameof(ParentOrganUnitId))]
        public OrganUnit ParentOrganUnit { get; set; }

        [CheckOnDelete("واحد سازمانی دارای زیرمجموعه می باشد و امکان حذف آن وجود ندارد")]
        public IList<OrganUnit> SubOrganUnits { get; set; }

        [CheckOnDelete("واحد سازمانی دارای پست سازمانی می باشد و امکان حذف آن وجود ندارد")]
        public IList<OrganPost> OrganPosts { get; set; }
    }
}
