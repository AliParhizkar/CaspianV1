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

        [DisplayName("عنوان")]
        public string Title { get; set; }

        [CheckOnDelete("نقش دارای عضو می باشد و امکان حذف آن وجود ندارد.")]
        public virtual ICollection<UserMembership> Memberships { get; set; }

        [CheckOnDelete("نقش دارای دستابی می باشد و امکان حذف آن وجود ندارد")]
        public virtual ICollection<UserAccessibility> Accessibilities { get; set; }
    }
}
