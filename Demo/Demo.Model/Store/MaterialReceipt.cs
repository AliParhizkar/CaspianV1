using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Demo.Model
{
    [Table("MaterialReceipt", Schema = "demo")]
    public class MaterialReceipt
    {
        [Key]
        public int Id { get; set; }

        public int ReceiptId { get; set; }

        [DisplayName("محصول")]
        public int MaterialId { get; set; }

        /// <summary>
        /// مشخصات حواله
        /// </summary>
        [ForeignKey(nameof(ReceiptId))]
        public virtual StoreHouseReceipt Receipt { get; set; }

        /// <summary>
        /// مشخصات محصول
        /// </summary>
        [ForeignKey(nameof(MaterialId))]
        public virtual Material Material { get; set; }

        /// <summary>
        /// واحد اصلی
        /// </summary>
        [DisplayName("واحد اصلی")]
        public int QuantityMain { get; set; }

        /// <summary>
        /// واحد فرعی
        /// </summary>
        [DisplayName("واحد فرعی")]
        public int? QuantitySub { get; set; }
    }
}
