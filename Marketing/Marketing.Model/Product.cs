using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Marketing.Model
{
    [Table("Products", Schema = "mrk")]
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("عنوان")]
        public string Title { get; set; }

        [DisplayName("قیمت")]
        public int Price { get; set; }

        [DisplayName("گروه محصول")]
        public int CategoryId { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public ProductCategory Category { get; set; }


    }
}
