using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Demo.Model
{
    [Table("CustomersGroups", Schema = "demo")]
    public class CustomerGroup
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// عنوان گروه
        /// </summary>
        [DisplayName("عنوان")]
        public string Title { get; set; }

        /// <summary>
        /// وضعیت فعالیت
        /// </summary>
        [DisplayName("وضعیت فعالیت")]
        public ActiveType ActiveType { get; set; }

        /// <summary>
        /// عضویت های گروه
        /// </summary>
        [CheckOnDelete("گروه مشتریان دارای عضویت می باشد و امکان حذف آن وجود ندارد")]
        public virtual IList<CustomerGroupMembership> CustomerGroupMemberships { get; set; }
    }
}
