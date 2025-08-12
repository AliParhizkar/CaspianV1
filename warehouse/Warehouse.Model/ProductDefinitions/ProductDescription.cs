using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Warehouse.Model
{
    [Table("ProductDescriptions", Schema = "wh")]
    public class ProductDescription
    {
        [Key]
        public int Id { get; set; }

        public int ProductId { get; set; }

        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; }

        [DisplayName("نام")]
        public string Name { get; set; }

        [DisplayName("نوع")]
        public DescriptionType DescriptionType { get; set; }

        [DisplayName("مقدار")]
        public string Value { get; set; }
    }
}
