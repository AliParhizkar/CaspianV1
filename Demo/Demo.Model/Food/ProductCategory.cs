using Caspian.Engine;
using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Demo.Model
{
    [Table("ProductCategories", Schema = "demo")]
    public class ProductCategory
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("Title"), ReportField]
        public string Title { get; set; }

        [DisplayName("Ordering")]
        public int Ordering { get; set; }

        [DisplayName("Code")]
        public string Code { get; set; }

        [DisplayName("Status")]
        public ActiveType ActiveType { get; set; }

        [CheckOnDelete("This product Category has Product and can not be removed")]
        public virtual IList<Product> Products { get; set; }
    }
}
