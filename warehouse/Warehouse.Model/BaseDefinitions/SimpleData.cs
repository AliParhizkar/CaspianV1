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

        public ICollection<StockRoom> StockRooms { get; set; }

        public ICollection<StockRoom> StockRooms1 { get; set; }
    }
}
