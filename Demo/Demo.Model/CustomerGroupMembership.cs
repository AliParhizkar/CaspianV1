using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Demo.Model
{
    [Table("CustomerGroupsMembership", Schema = "demo")]
    public class CustomerGroupMembership
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("Customer")]
        public int CustomerId { get; set; }

        [ForeignKey(nameof(CustomerId))]
        public Customer Customer { get; set; }

        [DisplayName("Customer Group")]
        public short CustomerGroupId { get; set; }

        [ForeignKey(nameof(CustomerGroupId))]
        public CustomerGroup CustomerGroup { get; set; }
    }
}
