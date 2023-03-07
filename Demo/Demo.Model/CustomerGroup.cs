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

        [DisplayName("Title")]
        public string Title { get; set; }

        [DisplayName("Status")]
        public ActiveType ActiveType { get; set; }

        [CheckOnDelete("Customer Group has Memberships and can not Removed")]
        public virtual IList<CustomerGroupMembership> CustomerGroupMemberships { get; set; }
    }
}
