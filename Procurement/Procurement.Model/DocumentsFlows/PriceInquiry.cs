using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Procurement.Model
{
    [Table("PriceInquiries", Schema = "pcm")]
    public class PriceInquiry
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("شماره استعلام")]
        public string InquiryNo { get; set; }

        [DisplayName("تاریخ استعلام")]
        public DateOnly InquiryDate { get; set; }

        [DisplayName("مهلت استعلام")]
        public DateOnly? InquiryDeadline { get; set; }

        [DisplayName("واحد تامین")]
        public int SupplyingUnitId { get; set; }

        [ForeignKey(nameof(SupplyingUnitId))]
        public SupplyingUnit SupplyingUnit { get; set; }

        [DisplayName("امکان ثبت چند پیش فاکتور برای هر تامین کننده")]
        public bool MultiplePreInvoice { get; set; }

        [DisplayName("توضیحات"), MaxLength(200)]
        public string Description { get; set; }

        [CheckOnDelete("استعلام دراای کالا می باشد و امکان حذف آن وجود ندارد")]
        public ICollection<PriceInquiryGoods> PriceInquiryGoods { get; set; }
    }
}
