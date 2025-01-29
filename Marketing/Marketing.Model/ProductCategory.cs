using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Marketing.Model
{
    [Table("ProductCategories", Schema = "mrk")]
    public class ProductCategory
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("Title")]
        public string Title { get; set; }

        [DisplayName("Ordering")]
        public int Ordering { get; set; }

        [DisplayName("Code")]
        public string Code { get; set; }

        [DisplayName("Status")]
        public ActiveType ActiveType { get; set; }

        //[CheckOnDelete("گروه دارای محصول می باشد و امکان حذف آن وجود ندارد")]
        //public IList<Product> Products { get; set; }
    }
}
