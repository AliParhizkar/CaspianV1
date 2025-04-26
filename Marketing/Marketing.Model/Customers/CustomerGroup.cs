using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Marketing.Model
{
    [Table("CustomerGroups", Schema = "mrk")]
    public class CustomerGroup
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("نام")]
        public string Name { get; set; }

        [CheckOnDelete("گروه دارای مشتری می باشد و امکان حذف آن وجود ندارد.")]
        public IList<Customer> Customers { get; set; }

        [CheckOnDelete("گروه دارای مشتری می باشد و امکان حذف آن وجود ندارد.")]
        public IList<CustomerGroupMembership> CustomerGroupMemberships { get; set; }
    }
}
