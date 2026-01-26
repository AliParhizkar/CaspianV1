using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Warehouse.Model
{
    [Table("Branches", Schema = "wh")]
    public class Branch
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("عنوان شعبه")]
        public string Title { get; set; }

        [CheckOnDelete("شعبه دارای مرکز نگهداری می باشد و امکان حذف آن وجود ندارد")]
        public ICollection<KeepCenter> KeepCenters { get; set; }

        [CheckOnDelete("شعبه دارای انبار می باشد و امکان حذف آن وجود ندارد")]
        public ICollection<StockRoom> Stocks { get; set; }
    }
}
