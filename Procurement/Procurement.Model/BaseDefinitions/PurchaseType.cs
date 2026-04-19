using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Procurement.Model
{
    public class PurchaseType
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("کد")]
        public string Code { get; set; }

        [DisplayName("عنوان")]
        public string Title { get; set; }

        [DisplayName("روند خرید")]
        public PurchaseProcessType PurchaseProcessType { get; set; }

        [DisplayName("توضیحات")]
        public string Description { get; set; }

        [DisplayName("فعال")]
        public bool IsActive { get; set; }
    }
}
