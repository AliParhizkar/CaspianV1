using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Marketing.Model
{
    [Table("ProductToppings", Schema = "mrk")]
    public class ProductTopping
    {
        [Key]
        public int Id { get; set; }

        public int ProductId { get; set; }

        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; }

        public int ToppingId { get; set; }

        [ForeignKey(nameof(ToppingId))]
        public Topping Topping { get; set; }
    }
}
