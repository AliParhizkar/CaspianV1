using Caspian.Engine;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Demo.Model
{
    [Table("OrderDeatil", Schema = "demo")]
    public class OrderDeatil
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("Order")]
        public int OrderId { get; set; }

        [ForeignKey(nameof(OrderId))]
        public Order Order { get; set; }

        [DisplayName("Descript")]
        public string Descript { get; set; }

        public int ProductId { get; set; }

        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; }

        [DisplayName("Price")]
        public int Price { get; set; }

        [DisplayName("Quantity")]
        public int Quantity { get; set; }

        [DisplayName("Sum"), DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public int Result { get;set; }
    }
}
