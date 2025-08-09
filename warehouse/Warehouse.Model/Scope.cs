using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Warehouse.Model
{
    [Table("Scopes", Schema = "wh")]
    public class Scope
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("عنوان")]
        public string Title { get; set; }

        [DisplayName("حوزه ی بالایی")]
        public int? ParentId { get; set; }

        [ForeignKey(nameof(ParentId))]
        public Scope Parent { get; set; }

        [CheckOnDelete("حوزه دارای زیر حوزه می باشد و امکان حذف آن وجود ندارد")]
        public ICollection<Scope> Scopes { get; set; }

        [CheckOnDelete("حوزه دارای انبار می باشد و امکان حذف آن وجود ندارد")]
        public ICollection<StockRoom> StockRooms { get; set; }
    }
}
