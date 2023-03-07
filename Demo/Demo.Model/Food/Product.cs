using Caspian.Common;
using Caspian.Engine;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Demo.Model
{
    [Table("Products", Schema = "demo")]
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("Title"), ReportField("عنوان محصول")]
        public string Title { get; set; }

        [DisplayName("Code")]
        public string Code { get; set; }

        [DisplayName("Product Category")]
        public int ProductCategoryId { get; set; }

        [DisplayName("Price"), ReportField]
        public int Price { get; set; }

        [DisplayName("Take out price"), ReportField]
        public int TakeoutPrice { get; set; }

        [DisplayName("Meal")]
        public Meal Meal { get; set; }

        [DisplayName("Out of stock")]
        public bool OutofStock { get; set; }

        [DisplayName("Discountable")]
        public bool Discountable { get; set; }

        [DisplayName("Status")]
        public ActiveType ActiveType { get; set; }

        [DisplayName("Image")]
        public byte[] Image { get; set; }

        [ForeignKey(nameof(ProductCategoryId)), ReportField("گروه محصول")]
        public virtual ProductCategory ProductCategory { get; set; }

        [CheckOnDelete("This product is ordered and cannot be removed")]
        public virtual IList<OrderDeatil> OrderDeatils { get; set; }
    }
}
