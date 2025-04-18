using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Marketing.Model
{
    [Table("Customer", Schema = "mrk")]
    public class Customer
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("نام")]
        public string Name { get; set; }

        [DisplayName("همراه")]
        public string MobileNumber { get; set; }

        [DisplayName("آدرس اول")]
        public string Address1 { get; set; }

        [DisplayName("آدرس دوم")]
        public string Address2 { get; set; }

        [DisplayName("گروه")]
        public int? GroupId { get; set; }

        [ForeignKey(nameof(GroupId))]
        public CustomerGroup CustomerGroup { get; set; }

        [CheckOnDelete("مشتری دارای آدرس می باشد و امکان حذف وی وجود ندارد")]
        public ICollection<CustomerAddress> Addresses{ get; set; }

        [CheckOnDelete("مشتری دارای سفارش می باشد و امکان حذف وی وجود ندارد")]
        public ICollection<Order> Orders { get; set; }

        [CheckOnDelete("مشتری عضو گروه می باشد و امکان حذف وی وجود ندارد")]
        public ICollection<CustomerGroupMembership> CustomerGroups { get; set; }
    }
}
