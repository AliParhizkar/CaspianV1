using Caspian.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Demo.Model
{
    [Table("AddressTypes", Schema = "demo")]
    public class AddressType
    {
        [Key]
        public int Id { get; set; }

        public string Title { get; set; }

        [CheckOnDelete("Address type has address and can not be removed")]
        public virtual IList<CustomerAddress> AddressTypes { get; set; }
    }
}
