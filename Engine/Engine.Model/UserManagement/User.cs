using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Engine.Model
{
    [Table("Users", Schema = "cmn")]
    public class User
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("Username")]
        public string UserName { get; set; }

        [DisplayName("First name")]
        public string FName { get; set; }

        [DisplayName("Last name")]
        public string LName { get; set; }

        [DisplayName("Password")]
        public string Password { get; set; }

        [DisplayName("Email")]
        public string Email { get; set; }

        [DisplayName("Mobile number")]
        public string MobileNumber { get; set; }

        [CheckOnDelete("The user is member of role and can not be removed")]
        public virtual ICollection<UserMembership> Memberships { get; set; }

        [CheckOnDelete("Th user has access to menus and can not be removed")]
        public virtual ICollection<MenuAccessibility> Accessibilities { get; set; }

        [CheckOnDelete("The user has some errors and can not be remoed")]
        public virtual ICollection<ExceptionDetail> ExceptionDetails { get; set; }
    }
}
