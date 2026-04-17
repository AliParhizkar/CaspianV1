using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Procurement.Model
{
    [Table("StockFlow", Schema = "wh")]
    public class StockFlow
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("تامین کننده")]
        public int? SupplierId { get; set; }

        [ForeignKey(nameof(SupplierId))]
        public Supplier Supplier { get; set; }

        [DisplayName("شماره")]
        public string No { get; set; }

        [DisplayName("شماره قرارداد")]
        public string ContractNo { get; set; }

        [DisplayName("تاریخ")]
        public DateOnly Date { get; set; }

    }
}
