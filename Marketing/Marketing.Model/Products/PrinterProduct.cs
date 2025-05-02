using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Marketing.Model
{
    [Table("PrinterProduct", Schema = "mrk")]
    public class PrinterProduct
    {
        [Key]
        public int Id { get; set; }

        public int ProductId { get; set; }

        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; }

        public int PrinterId { get; set; }

        [ForeignKey(nameof(PrinterId))]
        public PrinterLocation Printer {  get; set; }
    }
}
