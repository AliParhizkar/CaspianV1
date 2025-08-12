using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Warehouse.Model
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("گروه کالا")]
        public int CategoryId { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public ProductCategory ProductCategory { get; set; }

        [DisplayName("کد کالا")]
        public string Code { get; set; }

        [DisplayName("نام")]
        public string Name { get; set; }

        [DisplayName("کد استاندارد")]
        public string StandardCode { get; set; }

        [DisplayName("واحد کالا")]
        public int MaterialUnitId { get; set; }

        [ForeignKey(nameof(MaterialUnitId))]
        public MaterialUnit MaterialUnit { get; set; }

        [DisplayName("نقطه سفارش")]
        public decimal? OrderPoint { get; set; }

        [DisplayName("حداکثر")]
        public decimal? Maximum { get; set; }

        [DisplayName("نوع کالا")]
        public ProductType? ProductType { get; set; }

        [DisplayName("کد فنی")]
        public string TechnicalCode { get; set; }

        [DisplayName("زمان هشدار")]
        public int? WarningDate { get; set; }

        [DisplayName("قیمت استاندارد")]
        public int? StandardPrice { get; set; }

        [CheckOnDelete("کالا دارای ویژگی می باشد و امکان حذف آن وجود ندارد.")]
        public ICollection<ProductDescription> ProductDescriptions { get; set; }

        [CheckOnDelete("کالا دارای رسید می باشد و امکان حذف آن وجود ندارد")]
        public ICollection<ReceiptDetails> ReceiptDetails { get; set; }
    }
}
