using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Warehouse.Model
{
    [Table("PropertiesOfGoods", Schema = "wh")]
    public class PropertyOfGoods
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("ویژگی کالا")]
        public int GoodsPropertiesId { get; set; }

        [ForeignKey(nameof(GoodsPropertiesId))]
        public GoodsProperties GoodsProperty { get; set; }

        [DisplayName("کالا")]
        public int GoodsId { get; set; }

        [ForeignKey(nameof(GoodsId))]
        public Goods Goods { get; set; }

        public bool? BooleanField { get; set; }

        [MaxLength(500)]
        public string StringField { get; set; }

        public decimal? NumericField { get; set; }

        public DateOnly? DateField { get; set; }

        public int? PropertyListIdField { get; set; }

        [ForeignKey(nameof(PropertyListIdField))]
        public PropertyList PropertyList { get; set; }
    }
}
