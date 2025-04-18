using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Marketing.Model
{
    [Table("ProductDescriptions", Schema = "mrk")]
    public class ProductDescription
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("توضیحات"), MaxLength(255)]
        public string Description { get; set; }

        [DisplayName("محصول")]
        public int ProductId { get; set; }

        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; }
    }
}
