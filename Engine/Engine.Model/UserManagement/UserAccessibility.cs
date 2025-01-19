using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Engine.Model
{
    [Table("MenusAccessibility", Schema = "cmn")]
    public class MenuAccessibility
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("Menu")]
        public int MenuId { get; set; }

        [ForeignKey(nameof(MenuId))]
        public Menu Menu { get; set; }

        [DisplayName("User")]
        public int? UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; }

        [DisplayName("Role")]
        public int? RoleId { get; set; }

        [ForeignKey(nameof(RoleId))]
        public Role Role { get; set; }
    }
}
