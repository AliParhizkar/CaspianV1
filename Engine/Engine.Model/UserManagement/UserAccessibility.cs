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

        /// <summary>
        /// کد منو
        /// </summary>
        [DisplayName("منو")]
        public int MenuId { get; set; }

        /// <summary>
        /// مشخصات منو
        /// </summary>
        [ForeignKey(nameof(MenuId))]
        public virtual Menu Menu { get; set; }

        /// <summary>
        /// کد کاربر
        /// </summary>
        [DisplayName("کاربر")]
        public int? UserId { get; set; }

        /// <summary>
        /// مشخصات کاربر
        /// </summary>
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }

        /// <summary>
        /// کد نقش
        /// </summary>
        [DisplayName("نقش")]
        public int? RoleId { get; set; }

        /// <summary>
        /// مشخصات نقش
        /// </summary>
        [ForeignKey(nameof(RoleId))]
        public virtual Role Role { get; set; }
    }
}
