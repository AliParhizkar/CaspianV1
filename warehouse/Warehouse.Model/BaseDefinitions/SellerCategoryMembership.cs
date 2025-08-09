using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Warehouse.Model
{
    [Table("SellerCategoryMemberships", Schema = "wh")]
    public class SellerCategoryMembership
    {
        [Key]
        public int Id { get; set; }

        public int CategoryId { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public SellerCategory Category { get; set; }

        public int SellerId { get; set; }

        [ForeignKey(nameof(SellerId))]
        public Seller Seller { get; set; }
    }
}
