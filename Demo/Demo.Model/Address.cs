using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Demo.Model
{
    [Table("CustomersAddresses", Schema = "demo")]
    public class CustomerAddress
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("Address Type")]
        public int AddressTypeId { get; set; }

        [ForeignKey(nameof(AddressTypeId))]
        public AddressType AddressType { get; set; }

        [DisplayName("Customer")]
        public int CustomerId { get; set; }

        [ForeignKey(nameof(CustomerId))]
        public Customer Customer { get; set; }

        [DisplayName("Address")]
        public string Address {  get; set; }

        [DisplayName("Zipcode")]
        public string ZipCode { get; set; }

        [DisplayName("Active")]
        public bool IsActive { get; set; }

        [DisplayName("Default")]
        public bool IsDefault { get; set; }
    }
}
