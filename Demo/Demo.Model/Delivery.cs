using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Demo.Model
{
    [Table("couriers", Schema = "demo")]
    public class Courier
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("First name")]
        public string FName { get; set; }

        [DisplayName("Last name")]
        public string LName { get; set; }

        [DisplayName("Code")]
        public string Code { get; set; }

        [CheckOnDelete("courier has Orders and can not removed")]
        public virtual IList<Order> Orders { get; set; }    
    }
}
