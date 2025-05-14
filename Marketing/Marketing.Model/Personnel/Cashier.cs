using Caspian.Common;
using Caspian.Engine.Model;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Marketing.Model
{
    [Table("Cashier", Schema = "mrk")]
    public class Cashier
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("نام")]
        public string FirstName { get; set; }

        [DisplayName("نام خانوادگی")]
        public string LastName { get; set; }

        [DisplayName("جنسیت")]
        public Gender Gender { get; set; }

        [DisplayName("کاربری مرتبط")]
        public int RelatedUserId { get; set; }

        [ForeignKey(nameof(RelatedUserId))]
        public User User { get; set; }

        [DisplayName("وضعیت")]
        public ActiveType ActiveType { get; set; }

        [CheckOnDelete("صندوقدار دارای سفارش می باشد و امکان حذف وی وجود ندارد.")]
        public ICollection<Order> Orders { get; set; }
    }
}
