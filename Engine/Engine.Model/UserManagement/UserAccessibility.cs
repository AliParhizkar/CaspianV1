using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Engine.Model
{
    [Table("MenusAccessibility", Schema = "cmn")]
    public class MenuAccessibility
    {
        [Key]
        public int Id { get; set; }

        public int MenuId { get; set; }

        [ForeignKey(nameof(MenuId))]
        public virtual Menu Menu { get; set; }

        public int? UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }

        public int? RoleId { get; set; }

        [ForeignKey(nameof(RoleId))]
        public virtual Role Role { get; set; }
    }
}
