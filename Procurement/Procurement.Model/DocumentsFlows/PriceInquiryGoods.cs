using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Warehouse.Model;

namespace Procurement.Model
{
    [Table("PriceInquiriesGoods", Schema = "pcm")]
    public class PriceInquiryGoods
    {
        [Key]
        public int Id { get; set; }

        public int GoodsId { get; set; }

        [ForeignKey(nameof(GoodsId))]
        public Goods Goods { get; set; }

        public int PriceInquiryId { get; set; }

        [ForeignKey(nameof(PriceInquiryId))]
        public PriceInquiry PriceInquiry { get; set; }

        [DisplayName("نوع مبناء")]
        public ReferenceDocumentType ReferenceDocumentType { get; set; }


    }
}
