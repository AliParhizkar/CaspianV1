using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Warehouse.Model
{
    [Table("ProductCategories", Schema = "wh")]
    public class ProductCategory
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("عنوان")]
        public string Title { get; set; }

        [DisplayName("کد")]
        public string Code { get; set; }

        [DisplayName("شرح")]
        public string Description { get; set; }

        public int? CategoryId { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public ProductCategory Category { get; set; }

        [CheckOnDelete("گروه دارای زیرگروه می باشد و امکان حذف آن وجود ندارد")]
        public ICollection<ProductCategory> Categories { get; set; }

        [CheckOnDelete("گروه کالا دارای کالا می باشد و امکان حذف آن وجود ندارد")]
        public ICollection<Product> Products { get; set; }
    }
}
