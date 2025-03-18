using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Marketing.Model
{
    [Table("CustomersGroupMemberShip", Schema = "mrk")]
    public class CustomerGroupMemberShip
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("مشتری")]
        public int CustomerId { get; set; }


        [ForeignKey(nameof(CustomerId))]
        public Customer Customer { get; set; }

        public int GroupId { get; set; }

        [ForeignKey(nameof(GroupId))]
        public CustomerGroup Group { get; set; }
    }
}
