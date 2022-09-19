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

        [DisplayName("نام کاربری")]
        public string UserName { get; set; }

        [DisplayName("نام")]
        public string FName { get; set; }

        [DisplayName("نام خانوادگی")]
        public string LName { get; set; }

        [DisplayName("کلمه عبور")]
        public string Password { get; set; }

        [DisplayName("پست الکترونیکی")]
        public string Email { get; set; }

        [DisplayName("شماره همراه")]
        public string MobileNumber { get; set; }

        [CheckOnDelete("کاربر دارای عضویت می باشد و امکان حذف وی وجود ندارد")]
        public virtual ICollection<UserMembership> Memberships { get; set; }

        [CheckOnDelete("کاربر دارای دستیابی می باشد و امکان حذف وی وجود ندارد")]
        public virtual ICollection<MenuAccessibility> Accessibilities { get; set; }
    }
}
