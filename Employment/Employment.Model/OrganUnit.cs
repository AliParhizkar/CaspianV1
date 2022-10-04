using Caspian.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Employment.Model
{
    [Table("OrganUnits")]
    public class OrganUnit
    {
        [Key]
        public int Id { get; set; }

        public int? ParentOrganId { get; set; }

        [ForeignKey(nameof(ParentOrganId))]
        public virtual OrganUnit ParentOrgan { get; set; }

        public string Title { get; set; }

        public ActiveType ActiveType { get; set; }

        /// <summary>
        /// زیرواحدهای سازمانی واحد سازمانی
        /// </summary>
        [CheckOnDelete("واحد سازمانی داراری زیرواحد سازمانی می باشد و امکان حذف آن وجود ندارد")]
        public virtual ICollection<OrganUnit> SuborganUnits { get; set; }

        /// <summary>
        /// پستهای سازمانی واحد سازمانی
        /// </summary>
        [CheckOnDelete("واحد سازمانی دارای پست سازمانی می باشد و امکان حذف آن وجود ندارد")]
        public virtual ICollection<OrganPost> Posts { get; set; }
    }
}
