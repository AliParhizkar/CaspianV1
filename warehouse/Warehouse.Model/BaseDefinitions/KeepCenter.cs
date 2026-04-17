using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Warehouse.Model
{
    [Table("KeepCenters", Schema = "wh")]
    public class KeepCenter
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("کد")]
        public string Code { get; set; }

        [DisplayName("نام")]
        public string Name { get; set; }

        [DisplayName("محل جغرافیایی")]
        public int LocationId { get; set; }

        [ForeignKey(nameof(LocationId))]
        public Location Location { get; set; }

        [DisplayName("تلفن")]
        public string Tell { get; set; }

        [DisplayName("شعبه")]
        public int BranchId { get; set; }

        [DisplayName("کد پستی")]
        public string PostCode { get; set; }

        [DisplayName("آدرس"), MaxLength(200)]
        public string Address { get; set; }

        [ForeignKey(nameof(BranchId))]
        public Branch Branch { get; set; }

        [CheckOnDelete("مرکز نگهداری کالا دارای انبار می باشد و امکان حذف آن وجود ندارد")]
        public ICollection<StockRoom> Stocks { get; set; }

        [CheckOnDelete("مرکز نگهدارای کالا بعنوان محل رزرو کالا می باشد و امکان حذف آن وجود ندارد")]
        public ICollection<Reservation> Reservations { get; set; }

        [InverseProperty(nameof(StockFlow.KeepCenter))]
        [CheckOnDelete("مرکز نگهداری دارای گردش (مبداء) می باشد و امکان حذف آن وجود ندارد")]
        public ICollection<StockFlow> StockFlows { get; set; }

        [InverseProperty(nameof(StockFlow.OtherKeepCenter))]
        [CheckOnDelete("مرکز نگهداری دارای گردش (مقابل) می باشد و امکان حذف آن وجود ندارد")]
        public ICollection<StockFlow> OtherStockFlows { get; set; }
    }
}
