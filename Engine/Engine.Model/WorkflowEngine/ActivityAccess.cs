using Caspian.Common;
using Caspian.Common.Security;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Engine
{
    /// <summary>
    /// مشخصات دستیابی نقشها و کاربران به فعالیت های گردش ساز
    /// </summary>
    [Table("ActivitiesAccess", Schema = "cmn")]
    public class ActivityAccess
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Unique("کاربر به این فعالیت دسترسی دارد.", nameof(UserId), nameof(RoleId))]
        public int? ActivityId { get; set; }

        [ForeignKey(nameof(ActivityId))]
        public virtual Activity Activity { get; set; }

        public int? UserId { get; set; }

        public int? RoleId { get; set; }

        [ForeignKey(nameof(RoleId))]
        public virtual CustomRole Role { get; set; }
    }
}
