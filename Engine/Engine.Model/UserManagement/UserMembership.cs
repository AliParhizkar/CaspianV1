using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Engine.Model
{
    [Table("UsersMembership", Schema = "cmn")]
    public class UserMembership : IdentityUserRole<int>
    {
        [Key]
        public int Id { get; set; }

        public override int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; }

        public override int RoleId { get; set; }

        [ForeignKey(nameof(RoleId))]
        public Role Role { get; set; }
    }
}