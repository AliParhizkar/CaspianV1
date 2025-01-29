using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Demo.Model
{
    [Table("ProductsDescription", Schema = "Demo")]
    public class ProductDescription
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("محصول")]
        public int ProductId { get; set; }

        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; }

        public string Description { get; set; }
    }
}
