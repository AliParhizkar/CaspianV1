
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Marketing.Model
{
    [Table("MerchantsConfig")]
    public class MerchantConfig
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("پذیرنده")]
        public string Name { get; set; }

        [DisplayName("مشتری می تواند چندین آدرس داشته باشد")]
        public bool CustomerHasManyAddresses { get; set; }

        [DisplayName("مشتری عضو چندین گروه است")]
        public bool CustomerIsMemberOfGroups { get; set; }
    }
}
