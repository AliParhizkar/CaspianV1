using Caspian.Engine;
using Caspian.Engine.Model;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Warehouse.Model
{
    //[Table("Suppliers", Schema = "pcm11")]
    public class Supplier
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("کد")]
        public string Code { get; set; }

        [DisplayName("کاربر")]
        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; }

        [DisplayName("تاریخ شروع ارتباط")]
        public DateOnly? StartDate { get; set; }

        [DisplayName("نوع فعالیت")]
        public ActivityType ActivityType { get; set; }

        [DisplayName("فعال")]
        public bool IsActive { get; set; }

        [MaxLength(200), DisplayName("توضیحات")]
        public string Description { get; set; }
    }
}
