using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Marketing.Model
{
    [Table("CustomersAddress", Schema = "mrk")]
    public class CustomerAddress
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("مشتری")]
        public int CustomerId { get; set; }

        [ForeignKey(nameof(CustomerId))]
        public Customer Customer { get; set; }

        [DisplayName("آدرس")]
        public string Address {  get; set; }

        [DisplayName("آدرس پیش فرض")]
        public bool IsDefault { get; set; }

        [CheckOnDelete("سفارش با این آدرس ثبت شده است و امکان حذف آن وجود ندارد.")]
        public ICollection<Order> Orders { get; set; }
    }
}


