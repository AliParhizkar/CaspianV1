using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Warehouse.Model
{
    [Table("SimpleData", Schema = "wh")]
    public class SimpleData
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("عنوان")]
        public string Title { get; set; }

        [DisplayName("نوع")]
        public SimpleDataType DataType { get; set; }

        [CheckOnDelete("واحد مالی دارای انبار می باشد و امکان حذف آن وجود ندارد")]
        [InverseProperty(nameof(StockRoom.FinancialUnit))]
        public ICollection<StockRoom> StockRooms { get; set; }

        [CheckOnDelete("واحد بودجه دارای انبار می باشد و امکان حذف آن وجود ندارد")]
        [InverseProperty(nameof(StockRoom.BudgetUnit))]
        public ICollection<StockRoom> StockRooms1 { get; set; }
    }
}
