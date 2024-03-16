using Caspian.Common;
using System.ComponentModel;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Engine.Model
{
    [Table("Users", Schema = "cmn")]
    public class User : IdentityUser<int>
    {
        [Key]
        public override int Id { get; set; }

        [DisplayName("Username")]
        public override string UserName { get; set; }

        [DisplayName("First name")]
        public string FName { get; set; }

        [DisplayName("Last name")]
        public string LName { get; set; }

        [DisplayName("Password")]
        public string Password { get; set; }

        [DisplayName("Email")]
        public override string Email { get; set; }

        [DisplayName("Mobile number")]
        public string MobileNumber { get; set; }

        [NotMapped]
        public DateTime? ExpireDate { get; set; }

        [CheckOnDelete("The user is member of role and can not be removed")]
        public virtual ICollection<UserMembership> Memberships { get; set; }

        [CheckOnDelete("Th user has access to menus and can not be removed")]
        public virtual ICollection<MenuAccessibility> Accessibilities { get; set; }

        [CheckOnDelete("The user has some errors and can not be remoed")]
        public virtual ICollection<ExceptionDetail> ExceptionDetails { get; set; }
    }
}