using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Engine.Model
{
    [Table("Roles", Schema = "cmn")]
    public class Role
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("Title")]
        public string Title { get; set; }

        [CheckOnDelete("The role has members and it is not possible to delete it")]
        public virtual ICollection<UserMembership> Memberships { get; set; }

        [CheckOnDelete("The role has menus and it is not possible to delete it")]
        public virtual ICollection<MenuAccessibility> MenuAccessibilities { get; set; }
    }
}
