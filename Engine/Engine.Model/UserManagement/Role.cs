using Caspian.Common;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Engine.Model
{
    [Table("Roles", Schema = "cmn")]
    public class Role : IdentityRole<int>
    {
        [Key]
        public override int Id { get; set; }

        [DisplayName("Name")]
        public override string Name { get; set; }

        [CheckOnDelete("The role has members and it is not possible to delete it")]
        public virtual ICollection<UserMembership> Memberships { get; set; }

        [CheckOnDelete("The role has menus and it is not possible to delete it")]
        public virtual ICollection<MenuAccessibility> MenuAccessibilities { get; set; }
    }
}