using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Warehouse.Model
{
    public class SubstituteProduct
    {
        [Key]
        public int Id { get; set; }

        public int GoodsId { get; set; }

        [ForeignKey(nameof(GoodsId))]
        public Goods Goods { get; set; }

        public int SubstituteGoodsId { get; set; }

        [ForeignKey(nameof(SubstituteGoodsId))]
        public Goods SubstituteGoods { get; set; }

        [Precision(6, 3)]
        [DisplayName("نسبت جایگزینی")]
        public decimal Rate { get; set; }

        [DisplayName("رابطه عکس جایگزینی")]
        public bool DuplicateRelation { get; set; }
    }
}
