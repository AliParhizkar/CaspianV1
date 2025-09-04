using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Demo.Model
{
    [Table("AddressTypes", Schema = "demo")]
    public class AddressType
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("Title")]
        public string Title { get; set; }

        [CheckOnDelete("Address type has address and can not be removed")]
        public IList<CustomerAddress> AddressTypes { get; set; }
    }
}
