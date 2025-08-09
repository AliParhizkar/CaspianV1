using Caspian.Common;
using Caspian.Engine.Model;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Warehouse.Model
{
    [Table("StockRooms", Schema = "wh")]
    public class StockRoom
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("نام انبار")]
        public string Title { get; set; }

        [DisplayName("کد انبار")]
        public string Code { get; set; }

        [DisplayName("نوع انبار")]
        public StockRoomType StockRoomType { get; set; }

        [DisplayName("واحد مالی")]
        public int? FinancialUnitId { get; set; }

        [ForeignKey(nameof(FinancialUnitId))]
        public SimpleData FinancialUnit { get; set; }

        [DisplayName("واحد بودجه")]
        public int? BudgetUnitId { get; set; }

        [ForeignKey(nameof(BudgetUnitId))]
        public SimpleData BudgetUnit { get; set; }

        [DisplayName("مسئول انبار")]
        public int? ManagerId { get; set; }

        [ForeignKey(nameof(ManagerId))]
        public User Manager { get; set; }

        [DisplayName("حوزه")]
        public int? ScopeId { get; set; }

        [ForeignKey(nameof(ScopeId))]
        public Scope Scope { get; set; }

        [DisplayName("روش قیمت گذاری")]
        public PricingMethodType PricingMethodType { get; set; }

        [DisplayName("تلفن")]
        public string Tel { get; set; }

        [DisplayName("همراه")]
        public string Mobile { get; set; }

        [MaxLength(200), DisplayName("آدرس")]
        public string Address { get; set; }

        [MaxLength(200), DisplayName("شرح")]
        public string Description { get; set; }

        [CheckOnDelete("انبار دارای آدرس می باشد و امکان حذف آن وجود ندارد")]
        public ICollection<MaterialAddress> MaterialAddresses{ get; set; }
    }
}
