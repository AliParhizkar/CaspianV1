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

        [DisplayName("Materia")]
        public int MaterialId { get; set; }

        [ForeignKey(nameof(ReceiptId))]
        public virtual WarehouseReceipt Receipt { get; set; }

        [ForeignKey(nameof(MaterialId))]
        public virtual Material Material { get; set; }

        [DisplayName("Quantity-main")]
        public int QuantityMain { get; set; }

        [DisplayName("Quantity-sub")]
        public int? QuantitySub { get; set; }
    }
}
